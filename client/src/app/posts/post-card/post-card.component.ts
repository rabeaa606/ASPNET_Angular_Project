import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { take } from 'rxjs/operators';
import { Pagination } from 'src/app/_models/pagination';
import { Post } from 'src/app/_models/post';
import { PostLikes } from 'src/app/_models/postLikes';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { PostsService } from 'src/app/_services/posts.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PostLikesComponent } from 'src/app/modals/post-likes/post-likes.component';
import { Member } from 'src/app/_models/member';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-post-card',
  templateUrl: './post-card.component.html',
  styleUrls: ['./post-card.component.css']
})
export class PostCardComponent {
  @Input() post: Post;
  @Input() member: Member;

  @Output() deletePost = new EventEmitter();
  bsModalRef: BsModalRef;


  user: User;
  postLikes: PostLikes[] = []

  constructor(public accountService: AccountService,
    public postsService: PostsService,
    public adminService: AdminService,
    private modalService: BsModalService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }


  deletepost() {
    this.deletePost.emit(this.post);
  }
  async adminDeletepost() {
    await this.adminService.createHubConnection(this.user, this.post);
    this.deletePost.emit(this.post);
  }
  async addLike() {
    await this.postsService.createHubConnection(this.user, this.member, this.post);

    this.postsService.likeThread$?.subscribe(like => {
      if (like) this.postLikes.push(like);
    })
  }

  showLikes() {
    if (this.post) {
      this.postsService.getLikesOfPost(this.post.id).subscribe(likeslist => {
        this.postLikes = likeslist;
        const likes = this.postLikes;
        const config = {
          initialState: {
            likes
          }
        };
        this.bsModalRef = this.modalService.show(PostLikesComponent, config);
        this.bsModalRef.content.closeBtnName = 'Close';
      })

    }
  }

}
