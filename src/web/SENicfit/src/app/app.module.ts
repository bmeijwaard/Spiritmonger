import { DeckModule } from './main/deck/deck.module';
import { CardDialog } from './main/cards/dialog/card.dialog';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule, Routes } from '@angular/router';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { MatButtonModule, MatIconModule } from '@angular/material';
import { TranslateModule } from '@ngx-translate/core';
import 'hammerjs';

import { FuseModule } from '@fuse/fuse.module';
import { FuseSharedModule } from '@fuse/shared.module';
import { FuseProgressBarModule, FuseSidebarModule, FuseThemeOptionsModule } from '@fuse/components';

import { fuseConfig } from 'app/fuse-config';

import { AppComponent } from 'app/app.component';
import { LayoutModule } from 'app/layout/layout.module';
import { SampleModule } from 'app/main/sample/sample.module';
import { CardsModule } from './main/cards/cards.module';
import { environment } from 'environments/environment';
import { CardService } from './main/cards/cards.service';

const appRoutes: Routes = [
    {
        path: '**',
        redirectTo: 'deck'
    },
    {
        path: 'deck',
        loadChildren: './main/deck/deck.module#DeckModule'
    },
    {
        path: 'cards',
        loadChildren: './main/cards/cards.module#CardsModule'
    }
];

@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        RouterModule.forRoot(appRoutes, { useHash: true }),

        TranslateModule.forRoot(),

        // Material moment date module
        MatMomentDateModule,

        // Material
        MatButtonModule,
        MatIconModule,

        // Fuse modules
        FuseModule.forRoot(fuseConfig),
        FuseProgressBarModule,
        FuseSharedModule,
        FuseSidebarModule,
        FuseThemeOptionsModule,

        // App modules
        LayoutModule,
        SampleModule,
        CardsModule,
        DeckModule
    ],
    bootstrap: [AppComponent],
    entryComponents: [CardDialog],
    providers: [
        CardService,
        {
            provide: 'BASE_URL',
            useFactory: getBaseUrl
        }
    ]
})
export class AppModule {}

export function getBaseUrl(): string {
    return environment.baseUrl;
}
