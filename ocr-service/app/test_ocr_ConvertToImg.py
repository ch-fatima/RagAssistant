import fitz

PDF_PATH = r"E:\GFN\RagAssistant\ocr-service\samples\پایا و ساتنا و پل 1405.pdf"
OUTPUT_IMAGE = r"E:\GFN\RagAssistant\ocr-service\samples\page-1.png"

doc = fitz.open(PDF_PATH)

page = doc[0]

pix = page.get_pixmap(matrix=fitz.Matrix(2, 2))

pix.save(OUTPUT_IMAGE)

doc.close()

print("PDF converted successfully.")
print(f"Image saved to: {OUTPUT_IMAGE}")