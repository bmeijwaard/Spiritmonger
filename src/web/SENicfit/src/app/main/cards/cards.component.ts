import { CardDialog } from './dialog/card.dialog';
import { Component, ViewEncapsulation, ViewChild, ElementRef } from '@angular/core';
import { fuseAnimations } from '@fuse/animations';
import { DataSource } from '@angular/cdk/collections';
import { BehaviorSubject, Observable, merge, fromEvent, Subject } from 'rxjs';
import { MatPaginator, MatSort, _MatSortHeaderMixinBase, MatDialog } from '@angular/material';
import { map, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';
import { FuseUtils } from '@fuse/utils';
import { CardService } from './cards.service';
import { MetaCard } from 'app/_core/models/metacard.model';

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
