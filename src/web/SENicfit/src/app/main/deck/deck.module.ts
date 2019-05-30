import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';

import { FuseSharedModule } from '@fuse/shared.module';
import {
    MatAutocompleteModule,
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
import { DeckComponent } from './deck.component';


const routes = [
    {
        path: 'deck',
        component: DeckComponent
    }
];

@NgModule({
    declarations: [DeckComponent],
    imports: [
        RouterModule.forChild(routes),
        MatAutocompleteModule,
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
        MatSliderModule,
        MatSnackBarModule,
        MatSlideToggleModule,
        MatProgressSpinnerModule,
        MatTableModule,
        MatTabsModule,
        FuseSharedModule,
        FuseWidgetModule,
        FuseSharedModule
    ]
})
export class DeckModule {}
