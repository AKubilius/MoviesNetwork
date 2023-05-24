import Box from '@mui/material/Box/Box'
import React from 'react'
import { Link } from 'react-router-dom'

interface Movie {
    id: any;
    poster_path: any;
    createdDate: any;
    title: any;
    backdrop_path: any;

}

const ListMovie: React.FC<Movie> = ({
    id,
    poster_path,
    backdrop_path,
    createdDate,
    title,
}) => {
    return (
    <div className='collection'>
    <Box>
        <Box sx={{ display: 'flex', borderRadius: 2, width: '100%', border: 1 }}>
            <div className='movies'>
                <Box
                    component="img"
                    sx={{
                        margin: 1,
                        borderRadius: 2,
                        height: '90%',
                        width: '20%',
                    }}
                    alt="Movie"
                    src={`https://image.tmdb.org/t/p/original${poster_path}`}
                />
                <Box sx={{ display: 'flex', flexDirection: 'column', borderRadius: 2, width: '100%', marginTop: 1 }}>
                    <Link to={`/movie/${id}`} style={{color:'white'}}>{title}</Link>
                    <Box sx={{ borderRadius: 2, marginBottom: 0, display: 'flex', flexDirection: 'flex', alignItems: 'end', justifyContent: 'left', marginTop: 15 }}>
                       
                    </Box>
                </Box>
            </div>
        </Box>
    </Box>
</div>
  )
}

export default ListMovie