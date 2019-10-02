import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedDataService {

  private _uploadResponse = new BehaviorSubject(undefined);
  uploadResponse = this._uploadResponse.asObservable();

  constructor() { }

  setUploadResponse(response: any){
    this._uploadResponse.next(response);
  }

}
