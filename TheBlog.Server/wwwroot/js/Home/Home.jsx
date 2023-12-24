import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Loading from '../Shared/Loading';
import { useNavigate, Link } from 'react-router-dom';
import Paths from '../Shared/Paths';

const Home = () => {
    const [searchQuery, setSearchQuery] = useState('');
    const [lastFiveArticles, setLastFiveArticles] = useState([]);
    const [topThreeArticles, setTopThreeArticles] = useState([]);
    const [lastThreeCommentedArticles, setLastThreeCommentedArticles] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchArticles = async () => {
            setIsLoading(true);
            try {
                const response = await axios.get('/Home/Home');
                const data = response.data.homePageViewModel;
                setLastFiveArticles(data.lastFiveArticles || []);
                setTopThreeArticles(data.topThreeArticlesByRank || []);
                setLastThreeCommentedArticles(data.lastThreeCommentedArticles || []);
            } catch (error) {
                console.error('Error fetching articles:', error);
            }
            setIsLoading(false);
        };

        fetchArticles();
    }, []);

    const handleSearchChange = (e) => {
        setSearchQuery(e.target.value);
    };

    const handleSearch = () => {
        navigate(`${Paths.Articles}?title=${encodeURIComponent(searchQuery)}`);
    }

    if (isLoading) {
        return <Loading />;
    }

    return (
        <>
            <div className="container mt-4" style={{ maxWidth: '600px', marginBottom: '20px' }}>
                <div style={{ display: 'flex', justifyContent: 'space-between' }}>
                    <input
                        type="text"
                        placeholder="Search articles by title"
                        style={{ flexGrow: 1, marginRight: '10px' }}
                        onChange={handleSearchChange}
                    />
                    <button className="btn btn-secondary" onClick={handleSearch}>
                        Search
                    </button>
                </div>
            </div>

            <div className="container mt-4" style={{ display: 'flex', justifyContent: 'space-around', maxWidth: '1200px' }}>
                <div style={{ width: '30%' }}>
                    <h3 className="text-truncate">Last 5 Articles</h3>
                    <ul className="list-group">
                        {lastFiveArticles.map(article => (
                            <li key={article.id} className="list-group-item" style={{ marginBottom: '20px', borderTop: '1px solid rgba(0,0,0,.125)' }}> {/* Margin between li items */}
                                <Link to={Paths.Article.replace(":articleId", article.id)} className="text-truncate">
                                    <h5 className="text-truncate">{article.title}</h5>
                                </Link>
                                <p>Author: {article.author}</p>
                                <p>Rating: {article.rating}</p>
                            </li>
                        ))}
                    </ul>
                </div>

                <div style={{ width: '30%' }}>
                    <h3 className="text-truncate">Top 3 Articles</h3>
                    <ul className="list-group">
                        {topThreeArticles.map(article => (
                            <li key={article.id} className="list-group-item" style={{ marginBottom: '20px', borderTop: '1px solid rgba(0,0,0,.125)' }}> {/* Margin between li items */}
                                <Link to={Paths.Article.replace(":articleId", article.id)} className="text-truncate">
                                    <h5 className="text-truncate">{article.title}</h5>
                                </Link>
                                <p>Author: {article.author}</p>
                                <p>Rating: {article.rating}</p>
                            </li>
                        ))}
                    </ul>
                </div>

                <div style={{ width: '30%' }}>
                    <h3 className="text-truncate">Last 3 Commented Articles</h3>
                    <ul className="list-group">
                        {lastThreeCommentedArticles.map(article => (
                            <li key={article.id} className="list-group-item" style={{ marginBottom: '20px', borderTop: '1px solid rgba(0,0,0,.125)' }}> {/* Margin between li items */}
                                <Link to={Paths.Article.replace(":articleId", article.id)} className="text-truncate">
                                    <h5 className="text-truncate">{article.title}</h5>
                                </Link>
                                <p>Author: {article.author}</p>
                                <p>Rating: {article.rating}</p>
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </>
    );
}

export default Home;