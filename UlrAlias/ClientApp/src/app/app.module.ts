import {NgModule} from '@angular/core';
import {BrowserModule} from '@angular/platform-browser';
import {AppRoutingModule} from './app-routing.module';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import {HttpClientModule} from "@angular/common/http";
import {FormsModule} from '@angular/forms';

import {MatToolbarModule} from '@angular/material/toolbar';
import {MatButtonModule} from '@angular/material/button';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import {MatCardModule} from '@angular/material/card';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatNativeDateModule} from '@angular/material/core';

import {AppComponent} from './app.component';
import {HomeComponent} from './home/home.component';
import {InventoryComponent} from './inventory/inventory.component';
import {SwaggerComponent} from './swagger/swagger.component';

import {UrlShortenerService} from './services/url-shortener.service';
import {ThemeService} from "./services/theme.service";

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    InventoryComponent,
    SwaggerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    MatButtonModule,
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatSlideToggleModule,
    HttpClientModule
  ],
  providers: [
    UrlShortenerService,
    ThemeService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {
}
