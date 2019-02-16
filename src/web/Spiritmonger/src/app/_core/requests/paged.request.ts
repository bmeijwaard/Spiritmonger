export class PagedRequest {
  constructor(page: number, pagelength: number) {
    this.page = page;
    this.pageLength = pagelength;
  }
  page: number;
  pageLength: number;
}
