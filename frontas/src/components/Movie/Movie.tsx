import React, {useEffect, useState} from "react"
import "./Movie.css"
import { useParams } from "react-router-dom"
import { makeDeleteRequest, makePostRequest } from "../Api/Api";
import axios from "axios";
import Button from "@mui/material/Button/Button";
import DeleteIcon from '@mui/icons-material/Delete';
import AddCircleIcon from '@mui/icons-material/AddCircle';

const Movie = () => {
    const [currentMovieDetail, setMovie] = useState<any>([]);
    const { id } = useParams()


    const [inList, setInList] = useState(false);
    const handleClick = (Id: any) => {
     
      inList ?  deleteFromList(Id) :createRequest(Id)
      setInList(!inList);
    };
    const token = `Bearer ${sessionStorage.getItem("token")}`

    const fetchIsInList = async () => {
      const { data } = await axios.get(`https://localhost:7019/list/listedMovie/${id}`,
        {
          headers: {
            'Content-Type': 'application/json',
            Accept: 'application/json',
            Authorization: token
          },
        })
  
        setInList(data)
    }
    useEffect(() => {
      fetchIsInList()
    }, [])
  
    async function createRequest(Id: any) {
        const { data } = await makePostRequest('https://localhost:7019/List/Mylist', { MovieID: `${id}` });
        console.log(data)
        return data;
      }
    
      async function deleteFromList(Id: any) {
        const { data } = await makeDeleteRequest(`https://localhost:7019/List/${id}`);
    
        return data;
      }


    useEffect(() => {
        getData()
        window.scrollTo(0,0)
    }, [])

    const getData = () => {
        fetch(`https://api.themoviedb.org/3/movie/${id}?api_key=c9154564bc2ba422e5e0dede6af7f89b&language=lt-LT`)
        .then(res => res.json())
        .then(data => setMovie(data))
    }

 

    return (
        <div className="movie">
            <div className="movie__intro">
                <img className="movie__backdrop" src={`https://image.tmdb.org/t/p/original${currentMovieDetail ? currentMovieDetail.backdrop_path : ""}`} />
            </div>
            <div className="movie__detail">
                <div className="movie__detailLeft">
                    <div className="movie__posterBox">
                        <img className="movie__poster" src={`https://image.tmdb.org/t/p/original${currentMovieDetail ? currentMovieDetail.poster_path : ""}`} />
                    </div>
                </div>
                <div className="movie__detailRight">
                    <div className="movie__detailRightTop">
                        <div className="movie__name">{currentMovieDetail ? currentMovieDetail.title : ""}</div>
                        <div className="movie__tagline">{currentMovieDetail ? currentMovieDetail.tagline : ""}</div>
                        <div className="movie__rating">
                            {currentMovieDetail ? currentMovieDetail.vote_average: ""} <i className="fas fa-star" />
                            <span className="movie__voteCount">{currentMovieDetail ? "(" + currentMovieDetail.vote_count + ") votes" : ""}</span>
                        </div>  
                        <div className="movie__runtime">{currentMovieDetail ? currentMovieDetail.runtime + " mins" : ""}</div>
                        <div className="movie__releaseDate">{currentMovieDetail ? "Išleidimo data: " + currentMovieDetail.release_date : ""}</div>
                        <div className="movie__genres">
                            {
                                currentMovieDetail && currentMovieDetail.genres
                                ? 
                                currentMovieDetail.genres.map((genre: { id: string | undefined; name: string | number | boolean | React.ReactElement<any, string | React.JSXElementConstructor<any>> | React.ReactFragment | React.ReactPortal | null | undefined; }) => (
                                    <><span className="movie__genre" id={genre.id}>{genre.name}</span></>
                                )) 
                                : 
                                ""
                            }
                        </div>
                    </div>
                    <div className="movie__detailRightBottom">
                        <div className="synopsisText">Aprašymas 
                        <Button 
              onClick={() => handleClick({ id })} 
             
              startIcon={inList ? <DeleteIcon/>   :  <AddCircleIcon />} 
              variant="text"
              color={inList ? 'secondary' : 'primary' } 
            >
              {inList ? "Pašalinti" : "Pridėti" }
            </Button>

                        </div>
                        <div >{currentMovieDetail ? currentMovieDetail.overview : ""}</div>
                    </div>
                    
                </div>
            </div>
            <div className="movie__heading">Produkcijos kompanijos</div>
            <div className="movie__production">
                {
                    currentMovieDetail && currentMovieDetail.production_companies && currentMovieDetail.production_companies.map((company: { logo_path: string; name: string | number | boolean | React.ReactFragment | React.ReactPortal | React.ReactElement<any, string | React.JSXElementConstructor<any>> | null | undefined; }) => (
                        <>
                            {
                                company.logo_path 
                                && 
                                <span className="productionCompanyImage">
                                    <img className="movie__productionComapany" src={"https://image.tmdb.org/t/p/original" + company.logo_path} />
                                    <span>{company.name}</span>
                                </span>
                            }
                        </>
                    ))
                }
            </div>
        </div>
    )
}

export default Movie