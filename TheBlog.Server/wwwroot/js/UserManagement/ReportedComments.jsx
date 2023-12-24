import React, { useState, useEffect } from 'react';
import axios from 'axios';
import Loading from '../Shared/Loading';

const ReportedComments = () => {
    const [reportedComments, setReportedComments] = useState([]);
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        const fetchReportedComments = async () => {
            try {
                setIsLoading(true);
                const response = await axios.get('/ArticleComment/AllReportedComments');
                const data = response.data;
                console.log(data);

                if (data.reportedComments) {
                    setReportedComments(data.reportedComments);
                } else {
                    console.error('No reported comments found in the response.');
                }
            } catch (error) {
                console.error('Failed to fetch reported comments:', error);
            }
            finally {
                setIsLoading(false);
            }
        };

        fetchReportedComments();
    }, []);

    const handleBlockComment = async (reportedComment) => {
        console.log(reportedComment);
        const blockCommentViewModel = {
            ReportId: reportedComment.id,
            CommentId: reportedComment.commentId,
            BlockComment: true,
        };

        try {
            const response = await axios.patch('/ArticleComment/ChangeReportedCommentStatus', blockCommentViewModel);
            const data = response.data;

            if (data.success) {
                setReportedComments((prevComments) =>
                    prevComments.filter((comment) => comment.commentId !== reportedComment.commentId)
                );
            } else {
                console.error('Failed to block comment');
            }
        } catch (error) {
            console.error('Failed to block comment:', error);
        }
    };

    const handleDenyComment = async (reportedComment) => {
        console.log(reportedComment);
        const blockCommentViewModel = {
            ReportId: reportedComment.id,
            CommentId: reportedComment.commentId,
            BlockComment: false,
        };

        try {
            const response = await axios.patch('/ArticleComment/ChangeReportedCommentStatus', blockCommentViewModel);
            const data = response.data;

            if (data.success) {
                setReportedComments((prevComments) =>
                    prevComments.filter((comment) => comment.id !== reportedComment.id)
                );
            } else {
                console.error('Failed to block comment');
            }
        } catch (error) {
            console.error('Failed to block comment:', error);
        }
    };

    return (
        <div className="container text-center mt-5" style={{ maxWidth: '600px' }}>
            {isLoading ? (
                <Loading />
            ) : (
                <>
                    {reportedComments.length === 0 ? (
                        <div><h1>No reported comments</h1></div>
                    ) : (
                        <>
                            <h1>Reported Comments</h1>
                            {reportedComments.map((comment) => (
                                <div key={comment.id} className="text-left border p-3 my-3">
                                    <div>
                                        <strong>Reason For Report:</strong>
                                        <textarea
                                            className="form-control"
                                            value={comment.reason}
                                            rows={2}
                                        />
                                    </div>
                                    <div className="form-group mt-3">
                                        <label htmlFor={`commentText_${comment.id}`}><strong>Reported Comment:</strong></label>
                                        <textarea
                                            className="form-control"
                                            id={`commentText_${comment.id}`}
                                            value={comment.text}
                                            rows={4}
                                        />
                                    </div>
                                    <div className="mt-4">
                                        <button style={{ marginRight: '1rem', marginBottom: '1rem' }} className="btn btn-danger" onClick={() => handleBlockComment(comment)}>Block</button>
                                        <button style={{ marginBottom: '1rem' }} className="btn btn-warning" onClick={() => handleDenyComment(comment)}>Deny</button>
                                    </div>
                                </div>
                            ))}
                        </>
                    )}
                </>
            )}
        </div>
    );
};

export default ReportedComments;