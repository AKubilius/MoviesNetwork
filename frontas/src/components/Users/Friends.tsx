import React, { useState } from 'react'
import Avatar from '@mui/material/Avatar';
import PersonAddAlt1OutlinedIcon from '@mui/icons-material/PersonAddAlt1Outlined';
import PersonRemoveAlt1Icon from '@mui/icons-material/PersonRemoveAlt1';
import './User.css';
import Button from '@mui/material/Button';
import { makePostRequest, makeDeleteRequest } from "../Api/Api";
import { Link } from '@mui/material';

interface IUser {
  username: any;
  name: any;
  surname: any;
  id: any;
  image64: string;
}

const Friends: React.FC<IUser> = ({
  username,
  name,
  surname,
  image64,
  id
}) => {

 
  const sx = {
    margin: 2
  };

  return (
    <div style={{ display: 'flex' }}>
      <Avatar src={`data:image/jpeg;base64,${image64}`} sx={sx} variant="rounded" />
      <div className='user'>
        <h4 style={{
          marginBottom: 0

        }}> <Link href={`/profile/${username}`} style={{ textDecoration: 'none' }} > {username} </Link></h4>
        <h5 style={{
          marginTop: 0
        }}>{name} {surname}</h5>
      </div>
      
    </div>
  )
}

export default Friends