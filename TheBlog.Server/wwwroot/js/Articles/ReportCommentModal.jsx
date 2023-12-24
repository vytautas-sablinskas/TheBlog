import axios from 'axios';
import React, { useState, useEffect } from 'react';

const ReportCommentModal = ({ show, handleClose, handleSave, reportCommentIdToEdit }) => {
    const [description, setDescription] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [wordCountText, setWordCountText] = useState(`${description.length}/500`);
    const [isButtonDisabled, setIsButtonDisabled] = useState(description.length > 500 || description.lenght <= 0);

    useEffect(() => {
        if (show) {
            (async () => {
                try {
                    const response = await axios.get(`/ArticleComment/UserReportedCommentViewModel?commentId=${reportCommentIdToEdit}`);
                    const data = response.data.userReportedCommentViewModel;

                    if (data !== null) {
                        setDescription(data.reason || '');
                    } else {
                        console.log("not success");
                    }
                } catch (error) {
                    console.log("Failed");
                }
            })();
        }
    }, [reportCommentIdToEdit]);

    useEffect(() => {
        const wordCount = description.length;
        setWordCountText(`${wordCount}/500`);

        setIsButtonDisabled(wordCount === 0 || wordCount > 500);
    }, [description]);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!description) {
            setErrorMessage("Reason is required");
            return;
        }

        const reportCommentViewModel = {
            CommentId: reportCommentIdToEdit,
            Reason: description
        };

        try {
            const response = await axios.post('/ArticleComment/ReportComment', reportCommentViewModel);
            const data = response.data;

            if (data.success) {
                handleSave(reportCommentViewModel);
                handleClose();
            } else {
                setErrorMessage('Failed to report comment');
            }
        } catch (error) {
            setErrorMessage('Failed to report comment. Try again later!');
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
                        <h5 className="modal-title">Report Comment</h5>
                        <button type="button" className="close" onClick={handleClose}>
                            <span>&times;</span>
                        </button>
                    </div>
                    <div className="modal-body">
                        <form onSubmit={handleSubmit}>
                            <div className="form-group mb-3">
                                <label>Reason For Report</label>
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
                                Report Comment
                            </button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ReportCommentModal;