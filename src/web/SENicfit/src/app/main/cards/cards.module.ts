import { CardsComponent } from './cards.component';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FuseSharedModule } from '@fuse/shared.module';
import {
    MatButtonModule,
    MatChipsModule,
    MatExpansionModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatPaginatorModule,
    MatRippleModule,
    MatSelectModule,
    MatSortModule,
    MatSnackBarModule,
    MatTableModule,
    MatTabsModule,
    MatDialogModule,
    MatProgressBarModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatProgressSpinnerModule
} from '@angular/material';
import { FuseWidgetModule } from '@fuse/components';
import { CardService } from './cards.service';
import { CardDialog } from './dialog/card.dialog';

const routes = [
    {
        path: 'cards',
        component: CardsComponent
    }
];

@NgModule({
    declarations: [CardsComponent, CardDialog],
    imports: [
        RouterModule.forChild(routes),
        MatButtonModule,
        MatChipsModule,
        MatDialogModule,
        MatExpansionModule,
        MatFormFieldModule,
        MatIconModule,
        MatInputModule,
        MatPaginatorModule,
        MatProgressBarModule,
        MatRippleModule,
        MatSelectModule,
        MatSortModule,
        MatSnackBarModule,
        MatSlideToggleModule,
        MatProgressSpinnerModule,
        MatTableModule,
        MatTabsModule,
        FuseSharedModule,
        FuseWidgetModule,
        FuseSharedModule
    ],
    exports: [CardsComponent, CardDialog],
    providers: [CardService]
})
export class CardsModule {}
