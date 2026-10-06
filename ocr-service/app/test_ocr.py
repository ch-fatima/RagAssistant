from paddleocr import PaddleOCR
import fitz
import os
import time


PDF_PATH = r"E:\GFN\RagAssistant\ocr-service\samples\پایا و ساتنا و پل 1405.pdf"
OUTPUT_TEXT = r"E:\GFN\RagAssistant\ocr-service\samples\ocr-result.txt"
TEMP_IMAGE = r"E:\GFN\RagAssistant\ocr-service\samples\_ocr_page.png"


print("Creating OCR...")

ocr = PaddleOCR(
    lang="fa",
    use_doc_orientation_classify=False,
    use_doc_unwarping=False,
    use_textline_orientation=False,
    device="cpu",
    text_detection_model_name="PP-OCRv5_mobile_det",
    text_recognition_model_name="arabic_PP-OCRv5_mobile_rec"
)

print("OCR created successfully.")
print("Opening PDF...")

doc = fitz.open(PDF_PATH)

print(f"PDF opened successfully. Pages: {len(doc)}")

start = time.time()

with open(OUTPUT_TEXT, "w", encoding="utf-8") as output:

    for page_number in range(len(doc)):

        print(f"\nProcessing page {page_number + 1}/{len(doc)}...")

        page = doc[page_number]

        # Convert PDF page to image
        pix = page.get_pixmap(
            matrix=fitz.Matrix(2, 2)
        )

        pix.save(TEMP_IMAGE)

        # OCR
        page_start = time.time()

        result = ocr.predict(TEMP_IMAGE)

        print(
            f"OCR finished in "
            f"{time.time() - page_start:.2f} seconds."
        )

        output.write(
            f"\n===== صفحه {page_number + 1} =====\n\n"
        )

        # Extract recognized texts
        for page_result in result:

            texts = page_result.get("rec_texts", [])

            for text in texts:

                text = text.strip()

                if text:
                    output.write(text + "\n")

        # Delete temporary image
        if os.path.exists(TEMP_IMAGE):
            os.remove(TEMP_IMAGE)

doc.close()

print("\n================================")
print("OCR completed successfully.")
print(f"Result saved to:")
print(OUTPUT_TEXT)
print(f"Total time: {time.time() - start:.2f} seconds")
print("================================")