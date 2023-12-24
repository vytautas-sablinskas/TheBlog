const path = require('path');

module.exports = {
    context: path.resolve(__dirname, './'),
    entry: './wwwroot/js/App.jsx',
    output: {
        path: path.resolve(__dirname, './wwwroot/js'),
        filename: 'bundle.js',
    },
    mode: 'development',
    devtool: 'inline-source-map',
    module: {
        rules: [
            {
                test: /\.(js|jsx)$/,
                exclude: /node_modules/,
                use: {
                    loader: 'babel-loader',
                },
            },
        ],
    },
    resolve: {
        extensions: ['.js', '.jsx'],
        modules: [path.resolve(__dirname, 'node_modules'), 'node_modules'],
    },
};