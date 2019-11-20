import logging
import requests

import azure.functions as func


def main(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    '''

    create page url
    send html to it

    https://graph.microsoft.com/v1.0/me/onenote/sections/0-44CC1F8AB856A05A!110/pages

    <!DOCTYPE html>
<html>
  <head>
    <title>Test title</title>
    <meta name="created" content="2015-07-22T09:00:00-08:00" />
  </head>
  <body>
    <p>Test body</p>
  </body>
</html>

    {
        "title":email subject,
        "body":body

    }

    subject: note title
    body: note body

    Create new page in mind dump section, gps notebook, in one note. The title of the page will be
    email subject and body of page will be email body.


    '''

    title = req.params.get('title')
    body = req.params.get('body')
    if not title:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            title = req_body.get('title')

    note_html = "<!DOCTYPE html><html><head><title>{}</title></head><body><p>{}</p></body></html>".format(title, body)

    if title:
        requests.post("https://graph.microsoft.com/v1.0/me/onenote/sections/0-44CC1F8AB856A05A!110/pages", data=note_html)
        return func.HttpResponse("Note title: {} \n Note body: {}!".format(title, body))

        
    else:
        return func.HttpResponse(
             "Please pass a name on the query string or in the request body",
             status_code=400
        )
