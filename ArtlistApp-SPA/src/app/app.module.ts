import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';

import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { ProgressbarModule } from 'ngx-bootstrap';

import { FileInputComponent } from './file-input/file-input.component';
import { ThumbnailListComponent } from './thumbnail-list/thumbnail-list.component';
import { LoadingScreenComponent } from './loading-screen/loading-screen.component';
import { PathListComponent } from './path-list/path-list.component';

@NgModule({
   declarations: [
      AppComponent,
      HomeComponent,
      FileInputComponent,
      ThumbnailListComponent,
      LoadingScreenComponent,
      PathListComponent,
      PathListComponent
   ],
   imports: [
      BrowserModule,
      HttpClientModule,
      FormsModule,
      ProgressbarModule.forRoot()
   ],
   providers: [],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
