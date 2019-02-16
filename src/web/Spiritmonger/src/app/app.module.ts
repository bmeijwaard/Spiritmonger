import { AppService } from './app.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { environment } from 'src/environments/environment';
import { NgxMasonryModule } from 'ngx-masonry';

@NgModule({
  declarations: [AppComponent],
  imports: [BrowserModule, AppRoutingModule, ReactiveFormsModule, HttpClientModule, NgxMasonryModule
  ],
  providers: [
    AppService,
    {
      provide: 'BASE_URL',
      useFactory: getBaseUrl
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}

export function getBaseUrl(): string {
  return environment.baseUrl;
}
