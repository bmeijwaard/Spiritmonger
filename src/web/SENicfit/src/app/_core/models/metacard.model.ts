import { IMetaCardDTO } from './interfaces/metacard.interface';
import { MetaType } from './types/meta.type';

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

