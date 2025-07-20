import * as React from "react";
import { Upload, X, FileImage } from "lucide-react";
import { cn } from "../../lib/utils";

interface FileUploadProps {
  onFileSelect: (file: File | null) => void;
  selectedFile?: File | null;
  accept?: string;
  className?: string;
  placeholder?: string;
}

export const FileUpload = React.forwardRef<HTMLInputElement, FileUploadProps>(
  ({ onFileSelect, selectedFile, accept = "image/*", className, placeholder = "Upload ID Image" }, ref) => {
    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
      const file = event.target.files?.[0] || null;
      onFileSelect(file);
    };

    const removeFile = () => {
      onFileSelect(null);
      if (ref && 'current' in ref && ref.current) {
        ref.current.value = '';
      }
    };

    return (
      <div className={cn("w-full", className)}>
        <div className="relative">
          <input
            ref={ref}
            type="file"
            onChange={handleFileChange}
            accept={accept}
            className="absolute inset-0 w-full h-full opacity-0 cursor-pointer z-10"
          />
          <div className={cn(
            "border-2 border-dashed border-muted-foreground/25 rounded-lg p-6 text-center transition-colors",
            "hover:border-primary/50 hover:bg-muted/30",
            selectedFile && "border-primary bg-primary/5"
          )}>
            {selectedFile ? (
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <FileImage className="h-8 w-8 text-primary" />
                  <div className="text-left">
                    <p className="font-medium text-foreground">{selectedFile.name}</p>
                    <p className="text-sm text-muted-foreground">
                      {(selectedFile.size / 1024 / 1024).toFixed(2)} MB
                    </p>
                  </div>
                </div>
                <button
                  type="button"
                  onClick={removeFile}
                  className="text-muted-foreground hover:text-destructive transition-colors"
                >
                  <X className="h-5 w-5" />
                </button>
              </div>
            ) : (
              <div className="space-y-2">
                <Upload className="h-8 w-8 text-muted-foreground mx-auto" />
                <div>
                  <p className="font-medium text-foreground">{placeholder}</p>
                  <p className="text-sm text-muted-foreground">
                    Click to upload or drag and drop
                  </p>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    );
  }
);

FileUpload.displayName = "FileUpload";