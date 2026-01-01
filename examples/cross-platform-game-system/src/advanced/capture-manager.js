/**
 * Screenshot and Video Capture System
 * Phase 5: Advanced Features
 */

const fs = require('fs').promises;
const path = require('path');

class CaptureManager {
  constructor(options = {}) {
    this.capturePath = options.capturePath || path.join(require('os').homedir(), 'game-captures');
    this.videoQuality = options.videoQuality || 'high';
    this.screenshotFormat = options.screenshotFormat || 'png';
    this.recording = false;
  }

  /**
   * Initialize capture manager
   */
  async initialize() {
    await fs.mkdir(this.capturePath, { recursive: true });
    await fs.mkdir(path.join(this.capturePath, 'screenshots'), { recursive: true });
    await fs.mkdir(path.join(this.capturePath, 'videos'), { recursive: true });
  }

  /**
   * Take screenshot
   */
  async takeScreenshot(game) {
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    const filename = `${game.name}_${timestamp}.${this.screenshotFormat}`;
    const filepath = path.join(this.capturePath, 'screenshots', filename);
    
    // Placeholder: would use actual screenshot capture
    console.log(`📸 Screenshot saved: ${filepath}`);
    
    return {
      game: game.name,
      path: filepath,
      timestamp: new Date(),
      format: this.screenshotFormat
    };
  }

  /**
   * Start video recording
   */
  async startRecording(game) {
    if (this.recording) {
      throw new Error('Already recording');
    }
    
    const timestamp = new Date().toISOString().replace(/[:.]/g, '-');
    const filename = `${game.name}_${timestamp}.mp4`;
    const filepath = path.join(this.capturePath, 'videos', filename);
    
    this.recording = true;
    this.currentRecording = {
      game: game.name,
      path: filepath,
      startTime: new Date(),
      quality: this.videoQuality
    };
    
    console.log(`🎥 Recording started: ${filepath}`);
    
    return this.currentRecording;
  }

  /**
   * Stop video recording
   */
  async stopRecording() {
    if (!this.recording) {
      throw new Error('Not currently recording');
    }
    
    this.recording = false;
    const recording = this.currentRecording;
    recording.endTime = new Date();
    recording.duration = (recording.endTime - recording.startTime) / 1000;
    
    console.log(`🎥 Recording stopped: ${recording.duration}s`);
    
    this.currentRecording = null;
    
    return recording;
  }

  /**
   * List captures
   */
  async listCaptures(type = 'all') {
    const captures = {
      screenshots: [],
      videos: []
    };
    
    if (type === 'all' || type === 'screenshot') {
      const screenshotsPath = path.join(this.capturePath, 'screenshots');
      try {
        const files = await fs.readdir(screenshotsPath);
        captures.screenshots = files.map(f => ({
          name: f,
          path: path.join(screenshotsPath, f),
          type: 'screenshot'
        }));
      } catch (error) {
        // Directory might not exist
      }
    }
    
    if (type === 'all' || type === 'video') {
      const videosPath = path.join(this.capturePath, 'videos');
      try {
        const files = await fs.readdir(videosPath);
        captures.videos = files.map(f => ({
          name: f,
          path: path.join(videosPath, f),
          type: 'video'
        }));
      } catch (error) {
        // Directory might not exist
      }
    }
    
    return captures;
  }

  /**
   * Delete capture
   */
  async deleteCapture(capturePath) {
    await fs.unlink(capturePath);
    console.log(`🗑️  Deleted capture: ${capturePath}`);
    return true;
  }

  /**
   * Get recording status
   */
  isRecording() {
    return this.recording;
  }

  /**
   * Get current recording info
   */
  getCurrentRecording() {
    return this.currentRecording;
  }
}

module.exports = CaptureManager;
