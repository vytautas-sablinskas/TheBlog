import React, { useState } from 'react';
import axios from 'axios';

const DeleteArticleCommentModal = ({ show, handleClose, handleDelete, commentId }) => {
    const [errorMessage, setErrorMessage] = useState('');

    const handleCommentDelete = async () => {
        try {
            const response = await axios.delete(`/ArticleComment/ArticleComment/?commentId=${commentId}`);
            const data = response.data;

            if (data.success) {
                handleDelete(commentId);
                handleClose();
            } else {
                setErrorMessage("Failed deleting article. Try again later!");
            }
        } catch (error) {
            setErrorMessage("Error deleting article");
        }
    };

    if (!show) return null;

    return (
        <div className="modal show" style={{ display: 'block', backgroundColor: 'rgba(0, 0, 0, 0.5)' }}>
            <div className="modal-dialog modal-dialog-centered">
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
                        <h5 className="modal-title">Confirm Deletion</h5>
                        <button type="button" className="close" onClick={handleClose}>
                            <span>&times;</span>
                        </button>
                    </div>
                    <div className="modal-body">
                        Are you sure you want to delete this comment?
                    </div>
                    <div className="modal-footer">
                        <button type="button" className="btn btn-secondary" onClick={handleClose}>Cancel</button>
                        <button type="button" className="btn btn-danger" onClick={handleCommentDelete}>Delete</button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default DeleteArticleCommentModal;