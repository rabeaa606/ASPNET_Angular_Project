import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { PostLikes } from 'src/app/_models/postLikes';

@Component({
  selector: 'app-post-likes',
  templateUrl: './post-likes.component.html',
  styleUrls: ['./post-likes.component.css']
})
export class PostLikesComponent implements OnInit {
  @Input() updateSelectedRoles = new EventEmitter();
  likes: PostLikes[];

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

}
