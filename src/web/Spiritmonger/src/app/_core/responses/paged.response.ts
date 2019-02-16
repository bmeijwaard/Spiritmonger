export class PagedResponse<T> {
  page: number;
  pageLength: number;
  totalCount : number;
  data: Array<T>;
  namePart: string;
}
