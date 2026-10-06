import os
import tempfile

from fastapi import FastAPI, UploadFile, File, HTTPException

from app.ocr_service import OCRService


app = FastAPI(
    title="RagAssistant OCR Service",
    version="1.0.0"
)


ocr_service = OCRService()


@app.get("/")
def root():
    return {
        "service": "RagAssistant OCR Service",
        "status": "running"
    }


@app.post("/ocr")
async def ocr(file: UploadFile = File(...)):

    if not file.filename:
        raise HTTPException(
            status_code=400,
            detail="PDF file is required."
        )

    if not file.filename.lower().endswith(".pdf"):
        raise HTTPException(
            status_code=400,
            detail="Only PDF files are supported."
        )

    temp_pdf = tempfile.NamedTemporaryFile(
        suffix=".pdf",
        delete=False
    )

    temp_pdf_path = temp_pdf.name

    try:

        content = await file.read()

        temp_pdf.write(content)
        temp_pdf.close()

        text = ocr_service.extract_text(
            temp_pdf_path
        )

        if not text.strip():
            raise HTTPException(
                status_code=400,
                detail="No readable text found in PDF."
            )

        return {
            "fileName": file.filename,
            "text": text
        }

    finally:

        if os.path.exists(temp_pdf_path):
            os.remove(temp_pdf_path)