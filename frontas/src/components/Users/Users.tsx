import React from 'react'
import {useEffect, useState} from 'react'
import axios,{ AxiosRequestConfig } from 'axios'
import User from './User';
import UsersModal from './UsersModal';
import FriendsModal from './FriendsModal';
import { Box } from '@mui/material';

const Users = () => {
    const[users,setusers] = useState<any>([]);
    const token = `Bearer ${sessionStorage.getItem("token")}`
    const fetch = async () =>{
        
        const {data} = await axios.get(`https://localhost:7019/user`,
        {
            headers: {
                'Content-Type': 'application/json',
                Accept: 'application/json',
                Authorization: token
              },
        })
        setusers(data)

    }
    useEffect(()=>{
        fetch()
    }, [])

    
      const [sortedCompatibility, setSortedCompatibility] = useState([]);
    
      useEffect(() => {
        const fetchSortedCompatibility = async () => {
          try {
            const response = await axios.get("https://localhost:7019/List/sorted-compatibility", {
              headers: {
                "Content-Type": "application/json",
                "Authorization": token
              }
            });
    
            setSortedCompatibility(response.data.sortedCompatibility);
          } catch (error) {
            console.error("Error fetching sorted compatibility: ", error);
          }
        };
    
        fetchSortedCompatibility();
      }, []);

      console.log(sortedCompatibility)

  return (
    <div>
      {
        sortedCompatibility.length === 0 ? '' : <p>Siūlomi nariai</p>
      }
   

      {sortedCompatibility?.slice(0, 3).map((user: any, index: React.Key | null | undefined) => (
        <User
          username={user.userName}
          name={user.name}
          surname={user.surname}
          image64={user.profileImageBase64}
          id={user.id}
          key={user.id}
        />
      ))}

      <Box sx={{display:'flex',flexDirection:'row',justifyContent:'space-between'}}>
      <FriendsModal/>
      <UsersModal users={users}/>
      </Box>
      
    </div>
  )
}

export default Users