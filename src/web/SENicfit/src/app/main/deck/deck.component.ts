import { CardService } from './../cards/cards.service';
import { Component, ViewEncapsulation, ElementRef, ViewChild } from '@angular/core';
import { fuseAnimations } from '@fuse/animations';
import { FormControl, FormGroup, Validators, FormBuilder } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { startWith } from 'rxjs/internal/operators/startWith';
import { map } from 'rxjs/internal/operators/map';
import { List } from 'linqts';
import { MetaCard } from 'app/_core/models/metacard.model';

@Component({
    selector: 'deck',
    templateUrl: './deck.component.html',
    styleUrls: ['./deck.component.scss'],
    animations: fuseAnimations,
    encapsulation: ViewEncapsulation.None
})
export class DeckComponent {
    form: FormGroup;
    cardSelection: FormControl = new FormControl('', [Validators.required]);
    cardAmount: FormControl = new FormControl(null, [Validators.required, Validators.nullValidator]);
    deck: List<DeckCard> = new List<DeckCard>();
    total: number = 0;
    lands: number = 0;
    interaction: number = 0;
    manipulation: number = 0;
    finisher: number = 0;
    ramp: number = 0;
    averageCMC: number = 0;
    advantage: number = 0;

    options: string[] = [];
    filteredOptions: Observable<string[]>;
    subs: Subscription[] = new Array<Subscription>();

    @ViewChild('card') cardNameElement: ElementRef;

    constructor(private _cardService: CardService, private _formBuilder: FormBuilder) {
        _cardService.getCards();
    }

    ngOnInit(): void {
        this._reset();
        this.form = this._formBuilder.group({
            cardSelection: this.cardSelection,
            cardAmount: this.cardAmount
        });

        this.subs.push(
            this._cardService.cards$.subscribe(_ => {
                this.options = this._cardService.cards.map(card => card.name).sort();
            })
        );
    }

    ngOnDestroy(): void {
        this.subs.forEach(sub => sub.unsubscribe());
    }

    removeFromDeck(card: DeckCard) {
        this.deck.Remove(card);
        this._reset();
    }

    changeAmount(event: any, cardName: string) {
        this.deck.Where(c => c.name === cardName).ForEach(c => (c.amount = +event.target.value));
        this._reset();
    }

    addToDeck() {
        if (
            !this.form.valid ||
            !this.cardSelection.value ||
            !this.cardAmount.value ||
            !new List<string>(this.options).Any(v => v === <string>this.cardSelection.value)
        ) {
            return;
        }

        if (this.deck.Any(c => c.name === this.cardSelection.value)) {
            this.deck.Where(c => c.name === this.cardSelection.value).ForEach(c => (c.amount += +this.cardAmount.value));
        } else {
            this.deck.Add(<DeckCard>{
                name: this.cardSelection.value,
                amount: this.cardAmount.value
            });
        }
        // this.deck = this.deck.OrderBy(c => c.name);

        this._reset();
        this.cardNameElement.nativeElement.focus();
    }

    private _reset() {
        this.total = this.deck.Sum(c => c.amount);
        const selectedCards = new List<MetaCard>();
        this.deck.ForEach(deckCard => {
            selectedCards.AddRange(this._cardService.cards.filter(card => card.name === deckCard.name));
        });

        this.lands = 0;
        selectedCards.Where(c => c.isLand).ForEach(value => {
            this.lands += this.deck.Where(c => c.name == value.name).Sum(c => c.amount);
        });

        this.interaction = 0;
        selectedCards.Where(c => c.isInteraction).ForEach(value => {
            this.interaction += this.deck.Where(c => c.name == value.name).Sum(c => c.amount);
        });

        this.manipulation = 0;
        selectedCards.Where(c => c.isManipulation).ForEach(value => {
            this.manipulation += this.deck.Where(c => c.name == value.name).Sum(c => c.amount);
        });

        this.finisher = 0;
        selectedCards.Where(c => c.isFinisher).ForEach(value => {
            this.finisher += this.deck.Where(c => c.name == value.name).Sum(c => c.amount);
        });

        this.ramp = 0;
        selectedCards.Where(c => c.isRamp).ForEach(value => {
            this.ramp += this.deck.Where(c => c.name == value.name).Sum(c => c.amount);
        });

        this.averageCMC = 0;
        let totalCards = 0;
        selectedCards.Where(c => !c.isLand).ForEach(value => {
            const amount = this.deck.Where(c => c.name == value.name).Sum(c => c.amount);
            totalCards += amount;
            this.averageCMC += value.cmc * amount;
        });
        this.averageCMC = this.averageCMC / totalCards;

        this.advantage = 0;
        selectedCards.ForEach(value => {
            const amount = this.deck.Where(c => c.name == value.name).Sum(c => c.amount);
            this.advantage += value.cardAdvantage * amount;
        });

        this.cardSelection = new FormControl('', [Validators.required]);
        this.cardAmount = new FormControl(null, [Validators.required, Validators.nullValidator]);
        this.filteredOptions = this.cardSelection.valueChanges.pipe(
            startWith(''),
            map(value => this._filter(value))
        );
    }
    private _filter(value: string): string[] {
        const filterValue = value.toLowerCase();
        return this.options.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
    }
}

export interface DeckCard {
    name: string;
    amount: number;
}
