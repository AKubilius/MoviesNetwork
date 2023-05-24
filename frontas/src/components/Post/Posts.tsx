import React from 'react'
import axios, { AxiosRequestConfig } from 'axios'
import Post from './Post';
import CircularProgress from '@mui/material/CircularProgress';
import { useState, useEffect, useCallback, useRef } from 'react';
import { useParams } from 'react-router-dom';

interface Post {
  id: any;
  body: any;
  createdDate: any;
  imageUrl: any;
  movieId: any;
  user:User;

}

interface User {
  userName:string;
  profileImageBase64:string;
}

interface PostsProps {
  param: string | null;
}
const url = 'https://localhost:7019'


const Posts: React.FC<PostsProps>  = ({ param }) => {
  const requestInProgress = useRef(false);
  const token = `Bearer ${sessionStorage.getItem("token")}`
  const [posts, setPosts] = useState<any>([]);

  const {userName} = useParams();
  const [pages, setPage] = useState<number>(1);
  const [loading, setLoading] = useState<boolean>(false);

  const [totalPosts, setTotalPosts] = useState<number>(0);

  const fetchPosts = useCallback(() => {
    // Stop fetching if request in progress or all posts fetched
    if (requestInProgress.current || posts.length >= totalPosts) {
      return;
    }

    setLoading(true);
    requestInProgress.current = true;

    axios
      .get(`https://localhost:7019/Post${param}/${userName ? userName: ""}`, {
        params: {
          pageSize: 2,
          page: pages,
        },
        
          headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json',
            Authorization: token
          },
        
      })
      .then((response: { data: any[] }) => {
        const newPosts = response.data.filter((post: any) => !posts.some((p: any) => p.id === post.id));
        setPosts((prevPosts: any) => [...prevPosts, ...newPosts]);
        setPage((prevPage) => prevPage + 1);
        setLoading(false);
        requestInProgress.current = false;
      })
      .catch((error: any) => {
        console.log(error);
        requestInProgress.current = false;
      });
  }, [pages, posts, totalPosts]);


  const isMounted = useRef(true);


  const handleOnScroll = useCallback(() => {
    const windowHeight = window.innerHeight;
    const documentHeight = document.documentElement.scrollHeight;
    const scrollTop = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop || 0;

    if (!loading && scrollTop + windowHeight >= documentHeight - 100 && posts.length < totalPosts) {
      fetchPosts();
    }
  }, [loading, totalPosts, posts.length]);

  useEffect(() => {
    fetchPosts();
    axios
      .get('https://localhost:7019/Post/total',
      { headers: {
        'Content-Type': 'application/json',
        Accept: 'application/json',
        Authorization: token
      }})
      .then((response: { data: number }) => {
        setTotalPosts(response.data);
      })
      .catch((error: any) => console.log(error));
  }, [fetchPosts]);

  useEffect(() => {
    window.addEventListener('scroll', handleOnScroll);
    return () => {
      window.removeEventListener('scroll', handleOnScroll);
      isMounted.current = false;
    };
  }, [totalPosts, handleOnScroll]);

  return (
    <div>

      {posts?.map((post: any, index: React.Key | null | undefined) => (
        <Post
          id={post.id}
          body={post.body}
          imageUrl={post.imageUrl}
          movieId={post.movieId}
          createdDate={post.createdDate}
          key={index}
          image64={post.user.profileImageBase64}
        />
      ))}

    </div>
  )
}

export default Posts