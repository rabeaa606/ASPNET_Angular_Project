import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { Post } from 'src/app/_models/post';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { PostsService } from 'src/app/_services/posts.service';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.css']
})
export class AdminPanelComponent implements OnInit {


  posts: Partial<Post[]>;
  pageNumber = 1;
  pageSize = 5;
  otherpagination: Pagination;
  member: Member;
  user: User;
  constructor(private postsService: PostsService,
    private membersService: MembersService,
    public accountService: AccountService,
    private toastr: ToastrService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }
  ngOnInit(): void {
    this.loadMember();
    this.loadPosts();
  }
  loadPosts() {
    const core = "Others";
    this.postsService.getPosts(this.pageNumber, this.pageSize, core).subscribe(response => {
      this.posts = response.result;
      this.otherpagination = response.pagination;
    })
  }

  async loadMember() {
    await this.membersService.getMember(this.user.username).subscribe(member => {
      this.member = member;

    })
  }
  PageChanged(event: any) {
    this.pageNumber = event.page;
    this.loadPosts();
  }
  deletePost(post: Post) {
    this.posts.splice(this.posts.indexOf(post), 1)

  }

}