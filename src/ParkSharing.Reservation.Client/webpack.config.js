const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const { CleanWebpackPlugin } = require('clean-webpack-plugin');

module.exports = (env) => {
    return {
        entry: './src/index.js',
        output: {
            filename: 'bundle.js',
            path: path.resolve(__dirname, 'dist'),
            publicPath: '/'
        },
        module: {
            rules: [
                {
                    test: /\.(js|jsx)$/,
                    exclude: /node_modules/,
                    use: {
                        loader: 'babel-loader',
                    }
                },
                {
                    test: /\.css$/,
                    use: ['style-loader', 'css-loader']
                }
            ]
        },
        resolve: {
            extensions: ['.js', '.jsx']
        },
        plugins: [
            new CleanWebpackPlugin(),
            new HtmlWebpackPlugin({
                template: './public/index.html',
                filename: 'index.html'
            })
        ],
        devServer: {
            historyApiFallback: true,
            static: './', // Updated from contentBase to static
            hot: true,
            port: env.PORT || 4001,
            proxy: [
                {
                  context: ["/api"],  
                  target:
                    process.env.services__reservationserver__http__0,
                  pathRewrite: { "^/api": "" },
                  secure: false,
                },
              ],
        }
    }
};
