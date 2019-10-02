import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  uploadResponse: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
  }

  setUploadResponse(ev){
    this.uploadResponse = ev.items[0];
  }

}
