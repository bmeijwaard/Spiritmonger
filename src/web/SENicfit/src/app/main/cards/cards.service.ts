import { Injectable, Inject } from '@angular/core';
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { BaseService } from 'app/_core/services/base.service';
import { ApiResponse } from 'app/_core/models/responses/api.response';
import { MetaCard } from 'app/_core/models/metacard.model';
import { IMetaCardDTO } from 'app/_core/models/interfaces/metacard.interface';

@Injectable()
export class CardService extends BaseService implements Resolve<any> {
    cards: MetaCard[] = new Array<MetaCard>();
    onChanged: BehaviorSubject<MetaCard[]>;

    constructor(private _httpClient: HttpClient, @Inject('BASE_URL') protected readonly _baseUrl: string) {
        super(_httpClient, _baseUrl);
        this.onChanged = new BehaviorSubject(new Array<MetaCard>());
    }

    get cards$() {
        return this.onChanged.asObservable();
    }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<MetaCard[]> | Promise<MetaCard[]> | MetaCard[] {
        return new Promise((resolve, reject) => {
            Promise.all([this.getCards()]).then(() => {
                resolve();
            }, reject);
        });
    }

    getCards(): Promise<any> {
        return new Promise((resolve, reject) => {
            this._httpClient.get(`${this._baseUrl}metacard`).subscribe((response: ApiResponse<IMetaCardDTO>) => {
                this.cards = new Array<MetaCard>();
                ((<ApiResponse<IMetaCardDTO>>response).data as IMetaCardDTO[]).forEach(card => {
                    this.cards.push(new MetaCard(card));
                });
                this.onChanged.next(this.cards);
                resolve(response);
            }, reject);
        });
    }

    mutateCard(card: IMetaCardDTO): Promise<ApiResponse> {
        if (card.id) return this.put('metacard', card);
        else return this.post('metacard', card);
    }
}
