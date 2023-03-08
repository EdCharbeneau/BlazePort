const gulp = require('gulp');
const postcss = require("gulp-postcss");
const autoprefixer = require("autoprefixer");
const calc = require("postcss-calc");

// with gulp-sass 5
const sass = require('gulp-sass')(require('sass')); // use dart-sass

// An importer that redirects relative URLs starting with "~" to
// `node_modules`.
function packageImporter(url) {

    // If the doesn't start with ~, return it as is.
    if (!url.startsWith('~')) {
        return null;
    }

    const nodeModules = path.resolve('./node_modules');

    // Create a new url to the correct location
    const file = path.resolve(
        nodeModules,
        url.slice(1)
    );

    return { file };
}

const postcssPlugins = [
    calc({
        precision: 10
    }),
    autoprefixer({
        overrideBrowserslist: ['> 10%']
    })
];
const sassOptions = {
    precision: 10,
    outputStyle: 'expanded',
    importer: [packageImporter]
};

// Using gulp-sass 5
gulp.task('default', function () {
    return gulp.src('./Themes/site.scss')
        .pipe(sass(sassOptions).on('error', sass.logError))
        .pipe(postcss(postcssPlugins))
        .pipe(gulp.dest('./wwwroot/css'));
});