import { Injectable, Inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http/src/response';
import { BehaviorSubject } from 'rxjs';
import { CardSearchRequest } from './_core/requests/cardSearch.request';
import { ICard } from './_core/interfaces/card.interface';
import { HttpClient } from '@angular/common/http';
import { PagedResponse } from './_core/responses/paged.response';
import * as _ from 'lodash';

@Injectable()
export class AppService {
  private _pageLength: number = 24;
  protected _isLoading: BehaviorSubject<boolean>;
  private _cards: BehaviorSubject<Array<ICard>>;
  private _pagination: BehaviorSubject<CardSearchRequest>;
  private _dataStore: {
    cards: Array<ICard>;
    pagination: CardSearchRequest;
  };

  constructor(private readonly http: HttpClient, @Inject('BASE_URL') private readonly baseUrl: string) {
    this._isLoading = <BehaviorSubject<boolean>>new BehaviorSubject(false);
    this._dataStore = {
      cards: new Array<ICard>(),
      pagination: new CardSearchRequest('', 1, this._pageLength)
    };
    this._cards = <BehaviorSubject<Array<ICard>>>new BehaviorSubject(new Array<ICard>());
    this._pagination = <BehaviorSubject<CardSearchRequest>>new BehaviorSubject(new CardSearchRequest('', 1, this._pageLength));
  }

  protected async handleError(error: HttpErrorResponse): Promise<any> {
    return await Promise.reject(error);
  }

  get cards() {
    this._dataStore.cards = new Array<ICard>();
    this._cards = <BehaviorSubject<Array<ICard>>>new BehaviorSubject(new Array<ICard>());
    return this._cards.asObservable();
  }

  get pagination() {
    return this._pagination.asObservable();
  }

  get isLoading() {
    return this._isLoading.asObservable();
  }

  startSearch(namePart: string) {
    this._dataStore.pagination = new CardSearchRequest(namePart, 1, this._pageLength);
    this._dataStore.cards = new Array<ICard>();
    this._getCards();
  }

  nextPage() {
    this._dataStore.pagination.page++;
    this._getCards();
  }

  protected _handleError(error: any) {
    console.log(`error: ${error}`);
    this._isLoading.next(false);
  }

  private _getCards() {
    this._isLoading.next(true);
    this.http.post(this.baseUrl + 'card', this._dataStore.pagination).subscribe(
      data => {
        console.log('show me the money', data);
        if ((<any>data).success) {
          const response = (<any>data).data as PagedResponse<ICard>;
          this._dataStore.cards = this._dataStore.cards.concat(response.data);
          this._dataStore.cards = _.sortBy(this._dataStore.cards, card => (<ICard>card).relevance);
          this._dataStore.pagination = new CardSearchRequest(response.namePart, response.page, response.pageLength);

          this._cards.next(Object.assign({}, this._dataStore).cards);
          this._pagination.next(Object.assign({}, this._dataStore).pagination);

          this._isLoading.next(false);
        } else {
          this._handleError((<any>data).error);
        }
      },
      error => {
        this._handleError(JSON.stringify(error));
      }
    );
  }
}
