import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class RestClientService {
  baseUrl = 'http://localhost:5000/api/';
  
  constructor(private http: HttpClient) { }
  uploadFile(formData: any) {
    return this.http.post(this.baseUrl + 'videos/upload', formData, {reportProgress: true, observe: 'events'});
  }

}
