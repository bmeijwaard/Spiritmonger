import { Component, Inject } from '@angular/core';
import { IMetaCardDTO, MetaCard } from '../cards.component';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { CardService } from '../cards.service';

@Component({
    selector: 'card-dialog',
    templateUrl: 'card.dialog.html'
})
export class CardDialog {
    card: MetaCard = null;
    form: FormGroup;
    subs: Subscription[] = new Array<Subscription>();
    error: string = null;
    isLoading: boolean = null;

    constructor(
        public dialogRef: MatDialogRef<CardDialog>,
        @Inject(MAT_DIALOG_DATA) public data: any,
        private _formBuilder: FormBuilder,
        private _cardService: CardService
    ) {
        this.card = new MetaCard(data.card);
        this.subs.push(
            this._cardService.isLoading.subscribe(isLoading => {
                this.isLoading = isLoading;
            })
        );
    }

    ngOnInit(): void {
        this.form = this._formBuilder.group({
            name: [this.card.name, Validators.required],
            advantage: [this.card.cardAdvantage, Validators.required],
            cmc: [this.card.cmc, Validators.required],
            mana: [this.card.mana, Validators.maxLength(10)],
            manaCost: [this.card.manaCost, Validators.maxLength(10)],
            isInitial: [this.card.isInitial],
            isLand: [this.card.isLand],
            isInteraction: [this.card.isInteraction],
            isManipulation: [this.card.isManipulation],
            isFinisher: [this.card.isFinisher],
            isRamp: [this.card.isRamp]
        });
    }

    ngOnDestroy(): void {
        this.subs.forEach(sub => sub.unsubscribe());
    }

    onFormSubmit() {
        console.log(this.card.id);
        if (!this.form.valid) {
            return;
        }
        const res = Object.assign({}, this.form.value);
        let card = <IMetaCardDTO>{
            id: this.card.id,
            cardAdvantage: res.advantage,
            cmc: res.cmc,
            isInitial: res.isInitial,
            mana: res.mana.trim(),
            manaCost: res.manaCost.trim(),
            name: res.name.trim(),
            metaType: MetaCard.calculateMetaTypeFromForm(res)
        };
        this._cardService.mutateCard(card).then(response => {
            if (response.success) {
                this._cardService.getCards();
                this.dialogRef.close();
            } else {
                this.error = response.error;
                return;
            }
        });
    }
}
