import React, { useContext, useEffect, useState } from 'react';
import axios from 'axios';
import { useParams } from 'react-router-dom';
import Loading from '../Shared/Loading';
import PageNotFound from '../Shared/PageNotFound';
import { AuthContext } from '../Authentication/AuthProvider';
import Roles from '../Shared/Roles';
import EditArticleCommentModal from './EditArticleCommentModal';
import DeleteArticleCommentModal from './DeleteArticleCommentModal';
import ReportCommentModal from './ReportCommentModal';

const Article = () => {
    const { isAuthenticated, username, userRoles } = useContext(AuthContext);
    const [article, setArticle] = useState(null);
    const [comments, setComments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [submitCommentText, setSubmitCommentText] = useState('');
    const { articleId } = useParams();
    const [isEditModalVisible, setEditModalVisible] = useState(false);
    const [commentDescriptionToEdit, setCommentDescriptionToEdit] = useState('');
    const [commentIdToEdit, setCommentIdToEdit] = useState(-1);
    const [commentIdToDelete, setCommentIdToDelete] = useState(-1);
    const [isDeleteModalVisible, setDeleteModalVisible] = useState(false);
    const [isReportCommentModalVisible, setReportCommentModalVisible] = useState(false);
    const [commentIdToReport, setCommentIdToReport] = useState(-1);

    const openEditModal = (comment) => {
        setCommentDescriptionToEdit(comment.text);
        setCommentIdToEdit(comment.id);
        setEditModalVisible(true);
    };

    const closeEditModal = () => {
        setEditModalVisible(false);
    };

    const handleCommentDelete = (commentId) => {
        setComments(prevComments => prevComments.filter(comment => comment.id !== commentId));
    }

    const closeDeleteModal = () => {
        setDeleteModalVisible(false);
    }

    const openDeleteModal = (commentId) => {
        setCommentIdToDelete(commentId);
        setDeleteModalVisible(true);
    }

    const openReportCommentModal = (commentId) => {
        setCommentIdToReport(commentId);
        setReportCommentModalVisible(true);
    }

    const closeReportCommentModal = () => {
        setReportCommentModalVisible(false);
    }

    const handleCommentReport = () => {
        console.log("comment was reported");
    }

    const handleEditComment = (newComment) => {
        const updatedIndex = comments.findIndex((comment) => comment.id === newComment.CommentId);

        if (updatedIndex !== -1) {
            const updatedComments = [...comments];
            updatedComments[updatedIndex].text = newComment.Text;

            setComments(updatedComments);
        }
    };

    const handleSubmit = async () => {
        const viewModel = {
            ArticleId: articleId,
            Text: submitCommentText
        };

        const response = await axios.post(`/ArticleComment/ArticleComment`, viewModel);
        const data = response.data;

        if (data.success) {
            const newComment = {
                id: data.commentId,
                author: username,
                text: submitCommentText,
            };

            setComments([...comments, newComment]);
        } else {
            console.log("Failed submitting");
        }
    }

    useEffect(() => {
        const fetchArticle = async () => {
            try {
                const articleResponse = await axios.get(`/Article/Article?articleId=${articleId}`);
                const articleData = articleResponse.data.article;

                const articleFromData = {
                    author: articleData.author,
                    description: articleData.description,
                    id: articleData.id,
                    imageBase64Encoded: articleData.imageBase64Encoded,
                    title: articleData.title
                }

                console.log(articleData.comments);
                setArticle(articleFromData);
                setComments(articleData.comments);
                console.log(comments);
            } catch (err) {
                setError('Error fetching article');
            } finally {
                setLoading(false);
            }
        };

        if (articleId) {
            fetchArticle();
        }
    }, [articleId]);

    if (loading) {
        return (
            <Loading />
        );
    }

    if (error) {
        return <p>{error}</p>;
    }

    if (!article) {
        return (
            <PageNotFound />
        );
    }

    return (
        <div className="justify-content-center mt-4 mb-4">
            <div className="card" style={{ maxWidth: '500px', margin: '0 auto' }}>
                <div className="card-body text-center">
                    <h1 className="card-title">{article.title}</h1>
                </div>
                {article.imageBase64Encoded && (
                    <img
                        src={article.imageBase64Encoded}
                        className="img-fluid rounded mx-auto d-block"
                        alt="Article"
                        style={{ maxWidth: '80%' }}
                    />
                )}
                <div className="card-body">
                    {article.description && <p className="card-text">{article.description}</p>}
                </div>
            </div>

            <div className="mt-4" style={{ maxWidth: '500px', margin: '0 auto' }}>
                <h3 className="text-center">Comments</h3>
                {comments.map((comment) => (
                    <div key={comment.id} className="card mb-3">
                        <div className="card-body d-flex justify-content-between">
                            <div>
                                <h5 className="card-title">{comment.author}</h5>
                                <p className="card-text">{comment.text}</p>
                            </div>
                            <div className="d-flex flex-column">
                                {(comment.author === username || userRoles.includes(Roles.Admin)) && (
                                    <>
                                        <button className="btn btn-sm btn-primary mb-2" onClick={() => openEditModal(comment)}>Edit</button>
                                        <button className="btn btn-sm btn-danger mb-2" onClick={() => openDeleteModal(comment.id)}>Remove</button>
                                    </>
                                )}
                                {(comment.author !== username && isAuthenticated) && (
                                    <button className="btn btn-sm btn-warning mb-2" onClick={() => openReportCommentModal(comment.id)}>Report</button>
                                )}
                            </div>
                        </div>
                    </div>
                ))}
                {isAuthenticated && userRoles.includes(Roles.ArticleCommentator) && (
                    <div className="mt-4">
                        <small>
                            {submitCommentText.length}/500
                        </small>
                        <textarea
                            className="form-control mb-2"
                            placeholder="Write your comment here..."
                            value={submitCommentText}
                            onChange={(e) => {
                                if (e.target.value.length <= 500) {
                                    setSubmitCommentText(e.target.value);
                                }
                            }}
                        />
                        <button
                            className="btn btn-primary w-100"
                            onClick={handleSubmit}
                            disabled={submitCommentText.length > 500 || submitCommentText.length === 0}>
                            Submit Comment
                        </button>
                    </div>
                )}
            </div>
            <EditArticleCommentModal
                show={isEditModalVisible}
                handleClose={closeEditModal}
                handleSave={handleEditComment}
                initialDescription={commentDescriptionToEdit}
                commentIdToEdit={commentIdToEdit}
            />
            <DeleteArticleCommentModal
                show={isDeleteModalVisible}
                handleClose={closeDeleteModal}
                handleDelete={handleCommentDelete}
                commentId={commentIdToDelete}
            />
            <ReportCommentModal
                show={isReportCommentModalVisible}
                handleClose={closeReportCommentModal}
                handleSave={handleCommentReport}
                reportCommentIdToEdit={commentIdToReport}
            />
        </div>
    );
}

export default Article;