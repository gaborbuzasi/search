import pytesseract
from pdf2image import convert_from_path
import docx
import xlrd
import csv
import os
import json
import re

#def load_stop_words():
#   stop_files = [
#       '/var/lib/ocr/stopwords-de.txt',
#       '/var/lib/ocr/stopwords-en.txt',
#       '/var/lib/ocr/stopwords-it.txt']
#
#   stop_words = []
#
#   for stopf in stop_files:
#       fh = open(stopf, "r")
#       stop_content = fh.read()
#       fh.close()
#       stop_words += re.split(r"/|\n| |:", stop_content)
#
#
#def reduce_file(input_text):
#   load_stop_words()
#
#   input_text = input_text.lower()
#   input_text = re.sub('\n', ' ', input_text)
#   all_words = re.split(
#       r"/|\?|#|=|&|\. |\.\n,|\||!| |:|, | ,|\)|\(|- -|- | -|;", input_text)
#
#   return ' '.join(
#           list(set(
#               [w for w in all_words
#                if w not in ['','-','--','>','+','*']
#                and w not in stop_words])))


def get_json_output(location, filename, txt_content):
   content = {}
   content['folder'] = location
   content['filename'] = filename
   #content['text'] = reduce_file(txt_content)
   content['text'] = txt_content

   json.dump(content, outfile)


def pdf_to_txt(input_pdf):
    pages = convert_from_path(input_pdf)
    number = 0

    txt = ''
    for page in pages:
        number = number + 1
        txt = txt + pytesseract.image_to_string(page)

    return txt


def image_to_txt(input_image):
    page = convert_from_path(input_image)
    image_format = os.path.splitext(input_image)[-1].lower()

    # Check if it a PDF or TXT or CSV or Excel or JPEG or JPG or PNG TIF or GIF
    file_extension = ''
    if image_format == '.png':
        file_extension = 'PNG'
    elif image_format == '.jpeg' or image_format == 'jpg':
        file_extension = 'JPEG'
    elif image_format == '.tif':
        file_extension = 'TIF'
    else:
        print('The file extension are not supported')

    if file_extension is not '':
        txt = pytesseract.image_to_string(page)
        return txt
    else:
        print('Error')


def docx_to_text(input_docx):
    doc = docx.Document(input_docx)

    txt = ''

    for para in doc.paragraphs:
        txt += para.text.encode('ascii', 'ignore')

    return txt


def xls_to_txt(input_xls):
    x = xlrd.open_workbook(input_xls)
    # All table sheets ?
    x1 = [x.sheet_names()]

    input_csv = ''
    txt = ''

    for x2 in x1:
        for i in range(x2.nrows):
            input_csv += input_csv.writerow(x2.row_values(i))

        [txt.write(" ".join(row) + '\n') for row in csv.reader(input_csv)]

    return txt


def csv_to_txt(input_csv):
    txt = ''

    with open(input_csv, "r") as my_input_file:
        [txt.write(" ".join(row) + '\n') for row in csv.reader(my_input_file)]

    return txt


def find_files(root):
    for root, dirs, files in os.walk(root):
        for f in files:
            f = os.path.join(root, f)
            yield f

        for d in dirs:
            d = os.path.join(root, d)
            find_files(d)


def start_the_data_input_process(fp):
    location = os.path.dirname(fp)
    filename = os.path.basename(fp)
    ext = os.path.splitext(fp)[-1].lower()

    if ext == '.pdf':
        output = pdf_to_txt(fp)
    elif ext == '.docx':
        output = docx_to_text(fp)
    elif ext == '.csv':
        output = csv_to_txt(fp)
    elif ext == '.png' or ext == '.jpg' or ext == '.jpeg' or ext == '.tif':
        output = image_to_txt(fp)
    elif ext == '.xls' or ext == '.xlsx':
        output = xls_to_txt(fp)
    elif ext == '.db':
        print("Db Files as Thumpnails not working!")
        return None
    else:
        print("This extension not working!")
        return None

    return get_json_output(location, filename, output)
