/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { RestClientService } from './restClient.service';

describe('Service: RestClient', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RestClientService]
    });
  });

  it('should ...', inject([RestClientService], (service: RestClientService) => {
    expect(service).toBeTruthy();
  }));
});
