import { TestBed } from '@angular/core/testing';

import { LLMModelServiceService } from './llmmodel-service.service';

describe('LLMModelServiceService', () => {
  let service: LLMModelServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(LLMModelServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
