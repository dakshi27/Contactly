import { Component } from '@angular/core';
import { AsyncPipe } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Contact } from '../models/contact.model';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [AsyncPipe, FormsModule, ReactiveFormsModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  title = 'contactly.web';
  contacts$!: Observable<Contact[]>;
  loading = false;

  contactsForm = new FormGroup({
    name: new FormControl<string>(''),
    email: new FormControl<string | null>(null),
    phone: new FormControl<string>(''),
    favorite: new FormControl<boolean>(false)
  });

  constructor(private http: HttpClient) {
    this.refreshContacts();
  }

  onFormSubmit() {
    const addContactRequest = {
      name: this.contactsForm.value.name,
      email: this.contactsForm.value.email,
      phone: this.contactsForm.value.phone,
      favorite: this.contactsForm.value.favorite,
    };

    this.loading = true;

    this.http.post('https://localhost:7207/api/Contacts', addContactRequest)
      .subscribe({
        next: () => {
          this.contactsForm.reset();
          this.refreshContacts();
          this.loading = false;
        },
        error: (err) => {
          console.error('Error adding contact:', err);
          this.loading = false;
        }
      });
  }

  onDelete(id: string) {
    this.http.delete(`https://localhost:7207/api/Contacts/${id}`)
      .subscribe({
        next: () => {
          alert('Contact deleted');
          this.refreshContacts(); // Refresh list after deletion
        },
        error: (err) => {
          console.error('Error deleting contact:', err);
        }
      });
  }

  refreshContacts() {
    this.contacts$ = this.http.get<Contact[]>('https://localhost:7207/api/Contacts');
  }
}

















