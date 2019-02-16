import { PagedRequest } from './paged.request';

export class CardSearchRequest extends PagedRequest {
  constructor(namePart: string, page: number, pagelength: number){
    super(page, pagelength);
    this.namePart = namePart;
  }
  namePart: string;
}
