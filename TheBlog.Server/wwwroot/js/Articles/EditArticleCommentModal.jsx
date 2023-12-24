import axios from 'axios';
import React, { useState, useEffect } from 'react';

const EditArticleCommentModal = ({ show, handleClose, handleSave, initialDescription, commentIdToEdit }) => {
    const [description, setDescription] = useState(initialDescription);
    const [errorMessage, setErrorMessage] = useState('');
    const [wordCountText, setWordCountText] = useState(`${initialDescription.length}/500`);
    const [isButtonDisabled, setIsButtonDisabled] = useState(initialDescription.length > 500 || initialDescription.lenght <= 0);

    useEffect(() => {
        setDescription(initialDescription || '');
    }, [initialDescription]);

    useEffect(() => {
        const wordCount = description.length;
        setWordCountText(`${wordCount}/500`);

        setIsButtonDisabled(wordCount === 0 || wordCount > 500);
    }, [description]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!description) {
            setErrorMessage("Description is required");
            return;
        }

        const editArticleViewModel = {
            CommentId: commentIdToEdit,
            Text: description
        };

        try {
            const response = await axios.put('/ArticleComment/ArticleComment', editArticleViewModel);
            const data = response.data;

            console.log(data);
            if (data.success) {
                handleSave(editArticleViewModel);
                handleClose();
            } else {
                setErrorMessage('Failed to edit article');
            }
        } catch (error) {
            setErrorMessage('Failed to edit article. Try again later!');
        }
    };

    const handleDescriptionChange = (e) => {
        const inputText = e.target.value;

        if (inputText.length <= 500) {
            setDescription(inputText);
        }
    };

    if (!show) return null;

    return (
        <div className="modal show" style={{ display: 'block', backgroundColor: 'rgba(0, 0, 0, 0.5)' }}>
            <div className="modal-dialog modal-lg modal-dialog-centered">
                <div className="modal-content">
                    {errorMessage && (
                        <div className="row">
                            <div className="col-12 col-md-10 offset-md-1 text-center">
                                <div className="alert alert-danger mt-3" style={{ width: '95%', margin: '0 auto' }}>
                                    {errorMessage}
                                </div>
                            </div>
                        </div>
                    )}
                    <div className="modal-header">
                        <h5 className="modal-title">Edit Article</h5>
                        <button type="button" className="close" onClick={handleClose}>
                            <span>&times;</span>
                        </button>
                    </div>
                    <div className="modal-body">
                        <form onSubmit={handleSubmit}>
                            <div className="form-group mb-3">
                                <label>Description</label>
                                <textarea
                                    className="form-control"
                                    rows="5"
                                    value={description}
                                    onChange={handleDescriptionChange}
                                ></textarea>
                                <div className="text-right mt-2">
                                    <small>{wordCountText}</small>
                                </div>
                            </div>
                            <button type="submit" className="btn btn-primary w-100" disabled={isButtonDisabled}>
                                Edit Article
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default EditArticleCommentModal;