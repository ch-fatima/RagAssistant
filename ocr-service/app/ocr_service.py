import fitz
import os
import tempfile

from paddleocr import PaddleOCR


class OCRService:

    def __init__(self):
        print("Creating PaddleOCR...")

        self.ocr = PaddleOCR(
            lang="fa",
            use_doc_orientation_classify=False,
            use_doc_unwarping=False,
            use_textline_orientation=False,
            device="cpu",
            text_detection_model_name="PP-OCRv5_mobile_det",
            text_recognition_model_name="arabic_PP-OCRv5_mobile_rec"
        )

        print("PaddleOCR created successfully.")

    def extract_text(self, pdf_path: str) -> str:

        doc = fitz.open(pdf_path)

        all_text = []

        try:

            for page_number in range(len(doc)):

                print(
                    f"Processing page "
                    f"{page_number + 1}/{len(doc)}..."
                )

                page = doc[page_number]

                pix = page.get_pixmap(
                    matrix=fitz.Matrix(2, 2)
                )

                temp_image = tempfile.NamedTemporaryFile(
                    suffix=".png",
                    delete=False
                )

                temp_image_path = temp_image.name
                temp_image.close()

                try:

                    pix.save(temp_image_path)

                    result = self.ocr.predict(
                        temp_image_path
                    )

                    all_text.append(
                        f"===== صفحه {page_number + 1} ====="
                    )

                    for page_result in result:

                        texts = page_result.get(
                            "rec_texts",
                            []
                        )

                        for text in texts:

                            text = text.strip()

                            if text:
                                all_text.append(text)

                finally:

                    if os.path.exists(temp_image_path):
                        os.remove(temp_image_path)

        finally:

            doc.close()

        return "\n".join(all_text)