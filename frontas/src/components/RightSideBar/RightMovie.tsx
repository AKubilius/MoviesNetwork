import React from 'react'
import Box from '@mui/material/Box';
import { useNavigate } from 'react-router-dom';
import './Movie.css'

interface IPost {
    id: any;
    title: any;
    createdDate: any;
    posterPath: any;
    
}

const RightMovie: React.FC<IPost> = ({
    id,
    title,
    createdDate,
    posterPath
  }) => {

    const navigate = useNavigate();
const Url = "https://image.tmdb.org/t/p/original"


const handleClick = () => {
  
  navigate(`/movie/${id}`);
};

return (
    <div className='movies'>
      <Box
      
        component="img"
        sx={{
          margin: 1,
          borderRadius: 2,
          height: 1,
          width: '85%',
          cursor:'pointer'
        }}
        onClick={handleClick}
        alt="Movie"
        src={`${Url}${posterPath}`}
      />
    </div>
  )
}

export default RightMovie