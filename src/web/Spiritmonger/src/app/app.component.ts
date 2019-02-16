import { AppService } from './app.service';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable, Subscription } from 'rxjs';
import { Card } from './_core/models/card.model';
import { NgxMasonryOptions } from 'ngx-masonry';
import { ButtonHelper } from './_core/helpers/button.helper';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  card$: Observable<Array<Card>>;
  isLoading: boolean = null;
  loaderSub: Subscription;

  title = 'Spiritmonger';

  searchForm: FormGroup;
  namePart: FormControl;

  masonryOptions: NgxMasonryOptions = {
    transitionDuration: '0.2s',
    gutter: 0,
    resize: true,
    fitWidth: true,
    horizontalOrder: true,
    percentPosition: true,
    initLayout: true
  };

  constructor(private builder: FormBuilder, private cardService: AppService) {}

  ngOnInit() {
    this.createForm();
    this.card$ = this.cardService.cards;
    this.loaderSub = this.cardService.isLoading.subscribe(value =>  {
      this.isLoading = value;
      if(this.isLoading === false) {
        ButtonHelper.toggleButtonById('moreBtn', 'More');
        ButtonHelper.toggleButtonById('searchBtn', 'GO');
      } else {
        ButtonHelper.toggleButtonById('moreBtn');
        ButtonHelper.toggleButtonById('searchBtn');
      }
    });
  }

  ngOnDestroy() {
    this.loaderSub.unsubscribe();
  }

  showMore = (): void => {
    this.cardService.nextPage();
  };

  createForm() {
    this.namePart = new FormControl('', [Validators.required, Validators.minLength(4)]);
    this.searchForm = this.builder.group({
      namePart: this.namePart
    });
  }

  updateName = () => {
    this.namePart.markAsTouched();
    if (!this.searchForm.valid || this.isLoading) {
      return;
    }
    this.cardService.startSearch(this.namePart.value);
    this.title = this.namePart.value;
  };
}
