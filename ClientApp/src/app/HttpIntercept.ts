import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpEvent, HttpHandler, HttpRequest } from '@angular/common/http';

@Injectable()
export class HttpIntercept implements HttpInterceptor {
  constructor() { }

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    // Get the auth token from the service.
    const authToken = "ABC";

    // Clone the request and replace the original headers with
    // cloned headers, updated with the authorization.
    const authReq = req.clone({
      headers: req.headers.set('Authorization', authToken)
    });
    //if (!req.headers.has('Content-Type')) {
    //  req = req.clone({ headers: req.headers.delete('Content-Type', 'application/json') });
    //}

    // send cloned request with header to the next handler.
    return next.handle(authReq);
  }
}
