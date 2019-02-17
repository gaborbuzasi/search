FROM python:3.6.8-alpine3.9

RUN apk update && \
    apk --no-cache add build-base gcc gfortran zlib-dev jpeg-dev libxml2-dev \
                       libxslt-dev poppler-utils tesseract-ocr-dev
RUN mkdir -p /var/lib/ocr /var/log/ocr

COPY ./ocr /var/lib/ocr
COPY ./requirements.txt /var/lib/ocr/requirements.txt

RUN pip install --no-cache-dir -r /var/lib/ocr/requirements.txt

# automatize the data extraction and push the ingestion to ES
# at the moment the container is created
#
# WARNING! OCR processing takes several time!
# Consider to:
#      - Scale the OCR contanier (use more than one)
#      - Allocate as more resources as possible to the containers
#
CMD [ "python", "/var/lib/ocr/main.py" ]
