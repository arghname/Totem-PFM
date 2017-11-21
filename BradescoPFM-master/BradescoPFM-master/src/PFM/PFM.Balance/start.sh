#!/bin/bash
echo 'Compiling python function...'
python3 PFM.py
echo 'Python function compiled!'

echo 'Setting Flask initializing function'
export FLASK_APP=PFM.py
echo 'Set complete!'

echo 'Starting Flask server...'
flask run
