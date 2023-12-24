import React, { useContext, useEffect, useState } from 'react';
import { Link, useLocation } from 'react-router-dom';
import axios from 'axios';
import { AuthContext } from '../Authentication/AuthProvider';
import FetchLinks from '../Shared/FetchLinks';
import Paths from '../Shared/Paths';
import AddArticleModal from './AddArticleModal';
import EditArticleModal from './EditArticleModal';
import DeleteArticleModal from './DeleteArticleModal';
import Roles from '../Shared/Roles';

const Articles = () => {
    const { isAuthenticated, username, userRoles } = useContext(AuthContext);
    const [articles, setArticles] = useState([]);
    const [isLoading, setIsLoading] = useState(false);
    const [error, setError] = useState('');
    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [editArticleId, setEditArticleId] = useState(-1);
    const [deleteArticleId, setDeleteArticleId] = useState(-1);
    const [isSubmitting, setIsSubmitting] = useState(false);

    const location = useLocation();
    const searchParams = new URLSearchParams(location.search);
    const titleQueryParam = searchParams.get('title');
    const titleFilter = titleQueryParam === "null" || titleQueryParam === null ? "" : titleQueryParam;

    const handleSaveArticle = (title, id) => {
        const newArticle = {
            id: id,
            title: title,
            author: username,
            rating: 0,
            likedByUser: false,
            dislikedByUser: false
        };

        setArticles(prevArticles => [...prevArticles, newArticle]);
    };

    const handleEditArticle = (editedArticle) => {
        setArticles(prevArticles =>
            prevArticles.map(article =>
                article.id === editedArticle.Id ? { ...article, title: editedArticle.Title, description: editedArticle.Description } : article
            )
        );
    };

    const handleDelete = (articleId) => {
        setArticles(prevArticles => prevArticles.filter(article => article.id !== articleId));
    };

    const handleLike = async (articleId) => {
        if (isSubmitting) return;

        setIsSubmitting(true);
        const article = articles.find(a => a.id === articleId);

        if (!article) {
            setIsSubmitting(false);
            return;
        }

        const likedByUser = !article.likedByUser;
        const dislikedByUser = false;

        const viewModel = {
            articleId,
            likedByUser,
            dislikedByUser,
        };

        let data;
        try {
            let response;
            if (!article.likedByUser && !article.dislikedByUser) {
                response = await axios.post('/ArticleUserRating/Add', viewModel);
            } else if (article.dislikedByUser) {
                response = await axios.put('/ArticleUserRating/Update', viewModel)
            } else {
                response = await axios.delete('/ArticleUserRating/Delete', { params: { articleId } });
            }
            data = response.data;
        } catch (error) {
            console.error("Error updating article rating", error);
            setIsSubmitting(false);
            return;
        }

        if (!data.success) {
            setIsSubmitting(false);
            return;
        }

        setArticles(prevArticles =>
            prevArticles.map(article => {
                if (article.id === articleId) {
                    return {
                        ...article,
                        rating: likedByUser ? (article.dislikedByUser ? article.rating + 2 : article.rating + 1) : article.rating - 1,
                        likedByUser,
                        dislikedByUser
                    };
                }
                return article;
            })
        );

        setIsSubmitting(false);
    };

    const handleDislike = async (articleId) => {
        if (isSubmitting) return;

        setIsSubmitting(true);
        const article = articles.find(a => a.id === articleId);

        if (!article) {
            setIsSubmitting(false);
            return;
        }

        const likedByUser = false;
        const dislikedByUser = !article.dislikedByUser;

        const viewModel = {
            articleId,
            likedByUser,
            dislikedByUser,
        };

        let data;
        try {
            let response;
            if (!article.dislikedByUser && !article.likedByUser) {
                response = await axios.post('/ArticleUserRating/Add', viewModel);
            } else if (article.likedByUser) {
                response = await axios.put('/ArticleUserRating/Update', viewModel)
            } else if (article.dislikedByUser) {
                response = await axios.delete('/ArticleUserRating/Delete', { params: { articleId } });
            }
            data = response.data;
        } catch (error) {
            console.error("Error updating article rating", error);
            setIsSubmitting(false);
            return;
        }

        if (!data.success) {
            setIsSubmitting(false);
            return;
        }

        setArticles(prevArticles =>
            prevArticles.map(article => {
                if (article.id === articleId) {
                    return {
                        ...article,
                        rating: dislikedByUser ? (article.likedByUser ? article.rating - 2 : article.rating - 1) : article.rating + 1,
                        likedByUser,
                        dislikedByUser
                    };
                }
                return article;
            })
        );

        setIsSubmitting(false);
    };

    useEffect(() => {
        const fetchArticles = async () => {
            try {
                setIsLoading(true);
                const response = await axios.get(`${FetchLinks.Articles}?titleFilter=${titleFilter}`);
                setArticles(response.data.articles);
            } catch (err) {
                setError('Failed to load articles');
            } finally {
                setIsLoading(false);
            }
        };

        fetchArticles();
    }, []);

    if (isLoading) {
        return <p>Loading...</p>;
    }

    if (error) {
        return <p>{error}</p>;
    }

    return (
        <>
            <h1 className="text-center mt-4">Articles</h1>
            <div className="container mt-4" style={{ maxWidth: '600px' }}>
                {isAuthenticated && userRoles.includes(Roles.ArticleWriter) && (
                    <button className="btn btn-primary mb-3" onClick={() => setShowAddModal(true)}>Add Article</button>
                )}

                <div className="d-flex justify-content-between mb-2">
                    <span style={{ flex: 3 }}>Title</span>
                    <span style={{ flex: 2 }}>Author</span>
                    <span style={{ flex: 2 }}>Rating</span>
                    <span style={{ flex: 2 }}>Actions</span>
                </div>

                <ul className="list-group">
                    {articles.map((article) => (
                        <li key={article.id} className="list-group-item my-2 border-top">
                            <div className="d-flex justify-content-between align-items-center">
                                <Link to={Paths.Article.replace(":articleId", article.id)} className="text-truncate" style={{ flex: 3 }}>
                                    {article.title}
                                </Link>
                                <p style={{ flex: 2 }}>{article.author}</p>
                                <div style={{ flex: 2 }} className="d-flex align-items-center">
                                    {isAuthenticated && userRoles.includes(Roles.ArticleRater) ? (
                                        <>
                                            <button
                                                className={`btn ${article.likedByUser ? 'btn-success' : 'btn-outline-success'} btn-sm mx-1`}
                                                onClick={() => handleLike(article.id)}>+
                                            </button>
                                            <span>{article.rating}</span>
                                            <button
                                                className={`btn ${article.dislikedByUser ? 'btn-danger' : 'btn-outline-danger'} btn-sm mx-1`}
                                                onClick={() => handleDislike(article.id)}>-
                                            </button>
                                        </>
                                    ) : (
                                        <>
                                            <button className="btn btn-outline-success btn-sm mx-1" disabled>+</button>
                                            <span>{article.rating}</span>
                                            <button className="btn btn-outline-danger btn-sm mx-1" disabled>-</button>
                                        </>
                                    )}
                                </div>
                                {article.author === username || userRoles.includes(Roles.Admin) ? (
                                    <div style={{ flex: 2 }} className="d-flex">
                                        <button className="btn btn-secondary btn-sm me-1" onClick={() => { setEditArticleId(article.id); setShowEditModal(true); }}>Edit</button>
                                        <button className="btn btn-danger btn-sm" onClick={() => { setDeleteArticleId(article.id); setShowDeleteModal(true); }}>Remove</button>
                                    </div>
                                ) : (
                                    <div style={{ flex: 2 }}></div>
                                )}
                            </div>
                        </li>
                    ))}
                </ul>
            </div>
            <AddArticleModal
                show={showAddModal}
                handleClose={() => setShowAddModal(false)}
                handleSave={handleSaveArticle}
            />
            <EditArticleModal
                show={showEditModal}
                handleClose={() => setShowEditModal(false)}
                handleSave={handleEditArticle}
                selectedArticleId={editArticleId}
            />
            <DeleteArticleModal
                show={showDeleteModal}
                handleClose={() => setShowDeleteModal(false)}
                handleDelete={handleDelete}
                articleId={deleteArticleId}
            />
        </>
    );
};

export default Articles;