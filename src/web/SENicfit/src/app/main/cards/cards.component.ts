import { CardDialog } from './dialog/card.dialog';
import { Component, ViewEncapsulation, ViewChild, ElementRef } from '@angular/core';
import { fuseAnimations } from '@fuse/animations';
import { DataSource } from '@angular/cdk/collections';
import { BehaviorSubject, Observable, merge, fromEvent, Subject } from 'rxjs';
import { MatPaginator, MatSort, _MatSortHeaderMixinBase, MatDialog } from '@angular/material';
import { map, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FuseUtils } from '@fuse/utils';
import { CardService } from './cards.service';

@Component({
    selector: 'cards',
    templateUrl: './cards.component.html',
    styleUrls: ['./cards.component.scss'],
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})
export class CardsComponent {
    dataSource: CustomDataSource | null;
    displayedColumns = [
        'name',
        'cmc',
        'manaCost',
        'mana',
        'cardAdvantage',
        'isLand',
        'isInteraction',
        'isManipulation',
        'isFinisher',
        'isRamp'
    ];

    @ViewChild(MatPaginator)
    paginator: MatPaginator;

    @ViewChild(MatSort)
    sort: MatSort;

    @ViewChild('filter')
    filter: ElementRef;

    private _unsubscribeAll: Subject<any>;

    constructor(private _cardService: CardService, public dialog: MatDialog) {
        this._unsubscribeAll = new Subject();
    }

    ngOnInit(): void {
        this.dataSource = new CustomDataSource(this._cardService, this.paginator, this.sort);

        fromEvent(this.filter.nativeElement, 'keyup')
            .pipe(
                takeUntil(this._unsubscribeAll),
                debounceTime(150),
                distinctUntilChanged()
            )
            .subscribe(() => {
                if (!this.dataSource) return;
                this.dataSource.filter = this.filter.nativeElement.value;
            });
    }

    openDialog(id: string) {
        const card = this._cardService.cards.find(c => c.id == id) || new MetaCard(null);
        this.dialog.open(CardDialog, {
            data: {
                card: card
            }
        });
    }
}

export class CustomDataSource extends DataSource<any> {
    private _filterChange = new BehaviorSubject('');
    private _filteredDataChange = new BehaviorSubject('');

    constructor(private _cardService: CardService, private _matPaginator: MatPaginator, private _matSort: MatSort) {
        super();
        _cardService.getCards();
        this.filteredData = _cardService.cards;
    }

    connect(): Observable<any[]> {
        const displayDataChanges = [this._cardService.onChanged, this._matPaginator.page, this._filterChange, this._matSort.sortChange];

        return merge(...displayDataChanges).pipe(
            map(() => {
                let data = this._cardService.cards.slice();

                data = this.filterData(data);

                this.filteredData = [...data];

                data = this.sortData(data);

                // Grab the page's slice of data.
                const startIndex = this._matPaginator.pageIndex * this._matPaginator.pageSize;
                return data.splice(startIndex, this._matPaginator.pageSize);
            })
        );
    }

    get filteredData(): any {
        return this._filteredDataChange.value;
    }

    set filteredData(value: any) {
        this._filteredDataChange.next(value);
    }

    get filter(): string {
        return this._filterChange.value;
    }

    set filter(filter: string) {
        this._filterChange.next(filter);
    }

    filterData(data: MetaCard[]): MetaCard[] {
        if (!this.filter) {
            return data;
        }
        return FuseUtils.filterArrayByString(data, this.filter);
    }

    sortData(data: MetaCard[]): MetaCard[] {
        if (!this._matSort.active || this._matSort.direction === '') {
            return this._sort(data);
        }

        return this._sort(data, this._matSort.active, this._matSort.direction);
    }

    disconnect(): void {}

    private _sort(data: MetaCard[], column: string = 'name', direction: string = 'asc'): MetaCard[] {
        return data.sort((a, b) => {
            let propertyA: number | string = '';
            let propertyB: number | string = '';

            switch (column) {
                case 'id':
                    [propertyA, propertyB] = [a.id, b.id];
                    break;
                case 'name':
                    [propertyA, propertyB] = [a.name, b.name];
                    break;
                case 'cmc':
                    [propertyA, propertyB] = [a.cmc, b.cmc];
                    break;
                case 'manaCost':
                    [propertyA, propertyB] = [a.manaCost, b.manaCost];
                    break;
                case 'mana':
                    [propertyA, propertyB] = [a.mana, b.mana];
                    break;
            }

            const valueA = isNaN(+propertyA) ? propertyA : +propertyA;
            const valueB = isNaN(+propertyB) ? propertyB : +propertyB;

            return (valueA < valueB ? -1 : 1) * (direction === 'asc' ? 1 : -1);
        });
    }
}

export interface IMetaCardDTO {
    id: string;
    cardAdvantage: number;
    cmc: number;
    isInitial: boolean;
    mana: string;
    manaCost: string;
    metaType: number;
    name: string;
}

export class MetaCard implements IMetaCardDTO {
    constructor(data: IMetaCardDTO | undefined) {
        if (data) {
            this.id = data.id;
            this.cardAdvantage = data.cardAdvantage || 0;
            this.cmc = data.cmc;
            this.isInitial = data.isInitial;
            this.mana = data.mana;
            this.manaCost = data.manaCost;
            this.metaType = data.metaType;
            this.name = data.name;

            if (data.metaType & MetaType.LANDS) {
                this.isLand = true;
            }
            if (data.metaType & MetaType.INTERACTION) {
                this.isInteraction = true;
            }
            if (data.metaType & MetaType.MANIPULATION) {
                this.isManipulation = true;
            }
            if (data.metaType & MetaType.FINISHER) {
                this.isFinisher = true;
            }
            if (data.metaType & MetaType.RAMP) {
                this.isRamp = true;
            }
        } else {
            this.id = '';
            this.cardAdvantage = 0;
            this.cmc = 0;
            this.isInitial = false;
            this.mana = '';
            this.manaCost = '';
            this.metaType = 0;
            this.name = '';
        }
    }
    id: string;
    cardAdvantage: number;
    cmc: number;
    isInitial: boolean;
    mana: string;
    manaCost: string;
    metaType: number;
    name: string;

    isLand: boolean = false;
    isInteraction: boolean = false;
    isManipulation: boolean = false;
    isFinisher: boolean = false;
    isRamp: boolean = false;

    calculateMetaType(): number {
        let result = 0;
        if (this.isLand) result += 1;
        if (this.isInteraction) result += 2;
        if (this.isManipulation) result += 4;
        if (this.isFinisher) result += 8;
        if (this.isRamp) result += 16;
        return result;
    }

    static calculateMetaTypeFromForm(form: any): number {
        let result = 0;
        if (form.isLand) result += 1;
        if (form.isInteraction) result += 2;
        if (form.isManipulation) result += 4;
        if (form.isFinisher) result += 8;
        if (form.isRamp) result += 16;
        return result;
    }
}

export class MetaType {
    static readonly NONE = 0;
    static readonly LANDS = 1 << 0;
    static readonly INTERACTION = 1 << 1;
    static readonly MANIPULATION = 1 << 2;
    static readonly FINISHER = 1 << 3;
    static readonly RAMP = 1 << 4;
}
