import axios from 'axios';
import React, { useState } from 'react';

const EditArticleModal = ({ show, handleClose, handleSave, selectedArticleId }) => {
    const [articleData, setArticleData] = useState({
        title: '',
        description: '',
        image: null,
    });

    const [errorMessage, setErrorMessage] = useState('');

    const handleChange = (e) => {
        if (e.target.name === 'image') {
            setArticleData({ ...articleData, image: e.target.files[0] });
        } else {
            setArticleData({ ...articleData, [e.target.name]: e.target.value });
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (!articleData.title) {
            setErrorMessage("Title is required");
            return;
        }

        if (articleData.image && !['image/png', 'image/jpeg', 'image/gif'].includes(articleData.image.type)) {
            setErrorMessage("Only PNG, JPEG, and GIF files are allowed");
            return;
        }

        let base64Image = null;
        let imageType = '';
        if (articleData.image) {
            imageType = articleData.image.type.split('/')[1];
            base64Image = await convertToBase64(articleData.image);
            if (!base64Image) {
                setErrorMessage("Error in processing the image file");
                return;
            }
        }

        const editArticleViewModel = {
            Id: selectedArticleId,
            Title: articleData.title,
            Description: articleData.description || '',
            ImageBase64Encoded: base64Image,
            ImageType: imageType
        };

        try {
            const response = await axios.patch('/Article/Article', JSON.stringify(editArticleViewModel), {
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            const data = response.data;

            if (data.articleWasEdited) {
                handleSave(editArticleViewModel);
                handleClose();
            } else {
                setErrorMessage('Failed to edit article');
            }
        } catch (error) {
            setErrorMessage('Failed to edit article. Try again later!');
        }
    };

    const convertToBase64 = (file) => {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onloadend = () => {
                resolve(reader.result);
            };
            reader.onerror = reject;
            reader.readAsDataURL(file);
        });
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
                                <label>Title</label>
                                <input
                                    type="text"
                                    className="form-control"
                                    name="title"
                                    required
                                    value={articleData.title}
                                    onChange={handleChange}
                                />
                            </div>
                            <div className="form-group mb-3">
                                <label>Description</label>
                                <textarea
                                    className="form-control"
                                    rows="3"
                                    name="description"
                                    value={articleData.description}
                                    onChange={handleChange}
                                ></textarea>
                            </div>
                            <div className="form-group mb-3">
                                <label className="d-block mb-2">Upload Image:</label>
                                <input
                                    type="file"
                                    className="form-control-file d-block"
                                    accept="image/png, image/jpeg, image/gif"
                                    name="image"
                                    onChange={handleChange}
                                />
                            </div>
                            <button type="submit" className="btn btn-primary w-100/">Edit Article</button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default EditArticleModal;