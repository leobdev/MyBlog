let index = 0;

function addTag() {
    //get a reference to the TagEntry input element
    var tagEntry = document.getElementById("TagEntry");

    //new search function to help detect an error state
    let searchResult = search(tagEntry.value);
    if (searchResult != null) {
        //trigger my sweet alert for the empty string or whatever condition is contained in the searchResult var
        Swal.fire({
            title: 'Error!',
            text: 'Do toy want to continue?',
            icon: 'error',
            confirmButtonText: 'Cool'
        })
    }
    else {
        //create a new select option
        let newOption = new Option(tagEntry.value, tagEntry.value);
        document.getElementById("TagList").options[index++] = newOption;
    }

    //clear out the TagEntry control
    tagEntry.value = "";
    return true;
}








function search(str) {

    if (str == "") {
        return 'Empty tags are not permited';
    }

    var tagsElem = document.getElementById('TagList');

    if (tagsElem) {
        let options = tagsElem.options;
        for (let i = 0; i < options.length; i++) {
            if (options[i].value == str)
                return 'The tag #${str} was detected as a Duplicate tags and  not permitted';
        }
    }
}