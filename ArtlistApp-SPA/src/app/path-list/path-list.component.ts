import { Component, OnInit, Input } from '@angular/core';
import { SharedDataService } from '../_services/shared-data.service';

@Component({
  selector: 'app-path-list',
  templateUrl: './path-list.component.html',
  styleUrls: ['./path-list.component.css']
})
export class PathListComponent implements OnInit {
  uploadResponse: any;

  constructor(private sharedDataService: SharedDataService) { }

  ngOnInit() {
    this.sharedDataService.uploadResponse.subscribe(response => this.uploadResponse = response);
  }

}
