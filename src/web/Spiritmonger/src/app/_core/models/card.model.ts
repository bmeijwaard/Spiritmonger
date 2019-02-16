import { ICard } from './../interfaces/card.interface';
export class Card implements ICard {
  id: string;
  name: string;
  expansion: string;
  imageUrl: string;
  multiverseId: number;
  relevance: number;
}
