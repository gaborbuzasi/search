FROM python:3.6.8-alpine3.9

RUN apk update && \
    apk --no-cache add build-base gcc gfortran zlib-dev jpeg-dev libxml2-dev \
                       libxslt-dev poppler-utils tesseract-ocr-dev
RUN mkdir -p /var/lib/ocr

COPY ./ocr /var/lib/ocr
COPY ./requirements.txt /var/lib/ocr/requirements.txt

RUN pip install --no-cache-dir -r /var/lib/ocr/requirements.txt

#CMD [ "python", "/var/lib/ocr/main.py" ]
CMD [ "sleep", "10000" ]
