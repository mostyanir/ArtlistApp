import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { HttpClient, HttpEventType } from '@angular/common/http';

import { RestClientService } from '../_services/restClient.service';
import { LoadingScreenService } from '../_services/loading-screen.service';
import { AlertifyService } from '../_services/alertify.service';
import { SharedDataService } from '../_services/shared-data.service';


@Component({
  selector: 'app-file-input',
  templateUrl: './file-input.component.html',
  styleUrls: ['./file-input.component.css']
})
export class FileInputComponent implements OnInit {
  // tslint:disable-next-line: no-output-on-prefix
  @Output() onUploadFinished = new EventEmitter();
  public fileName = 'Choose file...';
  public progress: number;
  public message: string;

  public response;

  constructor(private restClientService: RestClientService,
              private loadingScreenService: LoadingScreenService,
              private sharedDataService: SharedDataService,
              private alertify: AlertifyService
    ) { }

  ngOnInit() {
  }

  public fileChangeEvent(fileInput: any) {
    if (fileInput.target.files && fileInput.target.files[0]) {
      this.fileName = fileInput.target.files[0].name;
      const reader = new FileReader();
    }
  }

  public uploadFile(files){
    if (files.length === 0) {
      this.alertify.error('Please selece file to upload');
      return;
    } else {
      this.loadingScreenService.startLoading();
    }
    const fileToUpload = <File>files[0];
    const formData = new FormData();
    formData.append('file', fileToUpload, fileToUpload.name);
 
    this.restClientService.uploadFile(formData)
      .subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          this.progress = Math.round(100 * event.loaded / event.total);
          console.log(this.progress);
        } else if (event.type === HttpEventType.Response) {
          this.response = event.body;
          this.message = 'Uploaded video successfully converted to the requested formats';
          this.loadingScreenService.stopLoading();
          this.alertify.success(this.message);

          //just to show more options of components communication
          this.sharedDataService.setUploadResponse(this.response.items[0]);
          this.onUploadFinished.emit(this.response);
        }
      });
  }

}
